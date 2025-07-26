using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Customer.GetCustomerQuery;
internal class GetCustomerQueryHandler
    : IRequestHandler<GetCustomerQuery, IResult<IReadOnlyList<GetCustomerQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Customer> _customerRepository;

    public GetCustomerQueryHandler(
        IRepository<InventorySystem_Domain.Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task<IResult<IReadOnlyList<GetCustomerQueryResponse>>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.Table
            .AsNoTracking()
            .Select(c => new GetCustomerQueryResponse(
                c.CustomerId,
                c.CustomerName,
                c.Phone,
                c.Address ?? string.Empty
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetCustomerQueryResponse>>.Success(customers);
    }
}
