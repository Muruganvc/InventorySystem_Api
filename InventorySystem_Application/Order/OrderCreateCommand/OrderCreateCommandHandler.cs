using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Order.OrderCreateCommand;
internal sealed class OrderCreateCommandHandler
    : IRequestHandler<OrderCreateCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    private readonly IRepository<InventorySystem_Domain.PaymentHistory> _paymentRepository;
    private readonly IUserInfo _userInfo;
    public OrderCreateCommandHandler(IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.Product> productRepository,
        IUserInfo userInfo,
        IRepository<InventorySystem_Domain.PaymentHistory> paymentRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _userInfo = userInfo;
        _paymentRepository = paymentRepository;
    }
    public async Task<IResult<int>> Handle(OrderCreateCommand request, CancellationToken cancellationToken)
    {
        int newOrderId = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // 1. Handle customer
            int customerId = request.Customer.CustomerId;
            if (customerId == 0)
            {
                var newCustomer = new InventorySystem_Domain.Customer
                {
                    CustomerName = request.Customer.CustomerName,
                    Address = request.Customer.Address,
                    Phone = request.Customer.Phone,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<InventorySystem_Domain.Customer>().AddAsync(newCustomer);
                await _unitOfWork.SaveAsync();
                customerId = newCustomer.CustomerId;
            }

            // 2. Create initial order
            var newOrder = new InventorySystem_Domain.Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                BalanceAmount = 0,
                FinalAmount = 0,
                TotalAmount = 0,
                IsGst = request.IsGst,
                GstNumber = request.GstNumber
            };

            await _unitOfWork.Repository<InventorySystem_Domain.Order>().AddAsync(newOrder);
            await _unitOfWork.SaveAsync();
            // 3. Add Order Items
            var orderItems = request.OrderItemRequests.Select(item =>
            {
                return new OrderItem
                {
                    OrderId = newOrder.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Meter > 0 ? 0 : item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercent = item.DiscountPercent,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _userInfo.UserId,
                    SerialNo = item.SerialNo,
                    Meter = item.Meter
                };
            }).ToList();

            await _unitOfWork.Repository<OrderItem>().AddRangeAsync(orderItems);
            await _unitOfWork.SaveAsync();
            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByAsync(p => p.ProductId == item.ProductId);
                if (product == null)
                    continue;

                bool isMeterBased = item.Meter > 0;

                if (isMeterBased)
                {
                    if (product.Meter >= item.Meter)
                    {
                        product.Meter -= item.Meter;
                        product.ModifiedBy = _userInfo.UserId;
                    }
                    else
                    {
                        Console.WriteLine($"Insufficient meter stock for ProductId: {item.ProductId}");
                    }
                }
                else
                {
                    if (product.Quantity >= item.Quantity)
                    {
                        product.Quantity -= item.Quantity;
                        product.ModifiedBy = _userInfo.UserId;
                    }
                    else
                    {
                        Console.WriteLine($"Insufficient quantity stock for ProductId: {item.ProductId}");
                    }
                }
            }

            await _unitOfWork.SaveAsync();

            // 4. Calculate totals
            var totalAmount = orderItems.Sum(i => i.Meter > 0 ? i.Meter * i.UnitPrice : i.Quantity * i.UnitPrice);
            var finalAmount = orderItems.Sum(i => i.Meter > 0 ? i.Meter * i.UnitPrice * (1 - i.DiscountPercent / 100.0m) : i.Quantity * i.UnitPrice * (1 - i.DiscountPercent / 100.0m));

            // 5. Update order totals
            newOrder.TotalAmount = totalAmount;
            newOrder.FinalAmount = finalAmount;
            newOrder.BalanceAmount = finalAmount - request.GivenAmount;

            var paymentHistory = InventorySystem_Domain.PaymentHistory.Create(newOrder.OrderId, customerId, request.GivenAmount, "Cash Payments", "", newOrder.BalanceAmount ?? 0, _userInfo.UserId);
            await _paymentRepository.AddAsync(paymentHistory);
            await _unitOfWork.SaveAsync();
            newOrderId = newOrder.OrderId;
            return newOrderId;
        }, cancellationToken);
        return Result<int>.Success(newOrderId);
    }
}