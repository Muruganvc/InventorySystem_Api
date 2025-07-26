using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Order.GetCustomerOrderSummaryQuery;

public record GetCustomerOrderSummaryQuery()
: IRequest<IResult<IReadOnlyList<GetCustomerOrderSummaryQueryResponse>>>;