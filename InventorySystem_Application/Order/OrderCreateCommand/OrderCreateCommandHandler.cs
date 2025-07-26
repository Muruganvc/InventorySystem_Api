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
    public OrderCreateCommandHandler(IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.Product> productRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
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
            var orderItems = request.OrderItemRequests.Select(item => new OrderItem
            {
                OrderId = newOrder.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercent = item.DiscountPercent,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                SerialNo = item.SerialNo
            }).ToList();

            await _unitOfWork.Repository<OrderItem>().AddRangeAsync(orderItems);
            await _unitOfWork.SaveAsync();
            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByAsync(p => p.ProductId == item.ProductId);
                if (product == null) continue;
                if (product.Quantity >= item.Quantity)
                {
                    product.Quantity -= item.Quantity;
                    product.ModifiedBy = 1;
                }
                else
                {
                    Console.WriteLine($"Insufficient stock for ProductId: {item.ProductId}"); //Log
                }
            }
            await _unitOfWork.SaveAsync();

            // 4. Calculate totals
            var totalAmount = orderItems.Sum(i => i.Quantity * i.UnitPrice);
            var finalAmount = orderItems.Sum(i => i.Quantity * i.UnitPrice * (1 - i.DiscountPercent / 100.0m));

            // 5. Update order totals
            newOrder.TotalAmount = totalAmount;
            newOrder.FinalAmount = finalAmount;
            newOrder.BalanceAmount = finalAmount - request.GivenAmount;

            // No need to call Update() explicitly if tracked by EF Core
            await _unitOfWork.SaveAsync(); // Single save for all changes
            newOrderId = newOrder.OrderId;
            return newOrderId;
        }, cancellationToken);
        return Result<int>.Success(newOrderId);
    }
}