using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Order.GetCustomerOrderSummaryQuery;
internal record class GetCustomerOrderSummaryQueryHandler
    : IRequestHandler<GetCustomerOrderSummaryQuery, IResult<IReadOnlyList<GetCustomerOrderSummaryQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Order> _orderRepository;
    private readonly IRepository<InventorySystem_Domain.Customer> _customerRepository;
    public GetCustomerOrderSummaryQueryHandler(
        IRepository<InventorySystem_Domain.Order> orderRepository,
        IRepository<InventorySystem_Domain.Customer> customerRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }
    public async Task<IResult<IReadOnlyList<GetCustomerOrderSummaryQueryResponse>>> Handle(GetCustomerOrderSummaryQuery request, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.Table
            .Join(
                _customerRepository.Table,
                ord => ord.CustomerId,
                cus => cus.CustomerId,
                (ord, cus) => new GetCustomerOrderSummaryQueryResponse(
                    ord.OrderId,
                    cus.CustomerName,
                    cus.Phone,
                    cus.Address!,
                    ord.OrderDate,
                    ord.TotalAmount ?? 0,
                    ord.FinalAmount ?? 0,
                    ord.BalanceAmount ?? 0
                )
            )
            .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<GetCustomerOrderSummaryQueryResponse>>.Success(result.OrderBy(a => a.CustomerName).ToList());
    }
}