using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Customer.GetCustomerQuery;
public record GetCustomerQuery(): IRequest<IResult<IReadOnlyList<GetCustomerQueryResponse>>>;
