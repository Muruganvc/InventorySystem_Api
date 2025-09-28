using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.PaymentHistory.GetPaymentHistoryQuery;

internal sealed class GetPaymentHistoryQueryHandler
    : IRequestHandler<GetPaymentHistoryQuery, IResult<IReadOnlyList<GetPaymentHistoryQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.PaymentHistory> _paymentHistoryRepository;
    private readonly IRepository<InventorySystem_Domain.Customer> _customerRepository;
    private readonly IRepository<InventorySystem_Domain.Order> _orderRepository;
    public GetPaymentHistoryQueryHandler(IRepository<InventorySystem_Domain.Customer> customerRepository,
        IRepository<InventorySystem_Domain.PaymentHistory> paymentHistoryRepository,
        IRepository<InventorySystem_Domain.Order> orderRepository)
    {
        _customerRepository = customerRepository;
        _paymentHistoryRepository = paymentHistoryRepository;
        _orderRepository = orderRepository;
    }
    public async Task<IResult<IReadOnlyList<GetPaymentHistoryQueryResponse>>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var result = await _paymentHistoryRepository.Table
                        .Where(p => p.OrderId == request.OrderId)
                        .Join(_customerRepository.Table,
                            p => p.CustomerId,
                            c => c.CustomerId,
                            (p, c) => new { p, c })
                        .Join(_orderRepository.Table,
                            pc => pc.p.OrderId,
                            o => o.OrderId,
                            (pc, o) => new GetPaymentHistoryQueryResponse(pc.c.CustomerName, o.FinalAmount ?? 0, pc.p.AmountPaid, pc.p.BalanceRemainingToPay,
                            pc.p.PaymentAt, pc.p.PaymentMethod ?? "N/A", pc.p.TransactionRefNo ?? string.Empty, ""))
                        .ToListAsync(cancellationToken);
            return Result<IReadOnlyList<GetPaymentHistoryQueryResponse>>.Success(result);
    }
}
