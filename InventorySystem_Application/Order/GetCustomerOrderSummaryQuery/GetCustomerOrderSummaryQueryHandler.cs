using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventorySystem_Application.Order.GetCustomerOrderSummaryQuery;
internal record class GetCustomerOrderSummaryQueryHandler
    : IRequestHandler<GetCustomerOrderSummaryQuery, IResult<IReadOnlyList<GetCustomerOrderSummaryQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Order> _orderRepository;
    private readonly IRepository<InventorySystem_Domain.Customer> _customerRepository;
    private readonly IRepository<InventorySystem_Domain.PaymentHistory> _paymentHistoryRepository;
    public GetCustomerOrderSummaryQueryHandler(
        IRepository<InventorySystem_Domain.Order> orderRepository,
        IRepository<InventorySystem_Domain.Customer> customerRepository,
        IRepository<InventorySystem_Domain.PaymentHistory> paymentHistoryRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _paymentHistoryRepository = paymentHistoryRepository;
    }
    public async Task<IResult<IReadOnlyList<GetCustomerOrderSummaryQueryResponse>>> Handle(
    GetCustomerOrderSummaryQuery request,
    CancellationToken cancellationToken)
    {
        // Step 1: Materialize the payment aggregates to memory
        var paymentAggregates = await _paymentHistoryRepository.Table.AsNoTracking()
            .GroupBy(ph => new { ph.OrderId, ph.CustomerId })
            .Select(g => new
            {
                g.Key.OrderId,
                g.Key.CustomerId,
                TotalPaid = g.Sum(x => x.AmountPaid),
                BalanceRemaining = g.OrderByDescending(x => x.PaymentAt).First().BalanceRemainingToPay,
                LastPaymentDate = g.Max(x => x.PaymentAt),
                LastPaymentMethod = g.OrderByDescending(x => x.PaymentAt).First().PaymentMethod
            })
            .ToListAsync(cancellationToken); // 🧠 Executed in memory

        // Step 2: Query orders and customers only
        var baseQuery = await _orderRepository.Table.AsNoTracking()
            .Join(
                _customerRepository.Table.AsNoTracking(),
                ord => ord.CustomerId,
                cus => cus.CustomerId,
                (ord, cus) => new { ord, cus }
            )
            .Select(x => new
            {
                x.ord.OrderId,
                x.ord.CustomerId,
                x.ord.OrderDate,
                x.ord.TotalAmount,
                x.ord.FinalAmount,
                x.ord.BalanceAmount,
                x.cus.CustomerName,
                x.cus.Phone,
                x.cus.Address
            })
            .ToListAsync(cancellationToken); // 🧠 Now all in-memory

        // Step 3: Merge with in-memory payment aggregates
        var result = baseQuery
            .Select(x =>
            {
                var payment = paymentAggregates
                    .FirstOrDefault(p => p.OrderId == x.OrderId && p.CustomerId == x.CustomerId);

                return new GetCustomerOrderSummaryQueryResponse(
                    x.OrderId,
                    x.CustomerName,
                    x.Phone,
                    x.Address!,
                    x.OrderDate,
                    x.TotalAmount ?? 0,
                    x.FinalAmount ?? 0,
                    payment?.BalanceRemaining ?? x.BalanceAmount ?? 0,
                    x.CustomerId
                );
            })
            .OrderBy(x => x.CustomerName)
            .ToList();

        return Result<IReadOnlyList<GetCustomerOrderSummaryQueryResponse>>.Success(result);
    }



}