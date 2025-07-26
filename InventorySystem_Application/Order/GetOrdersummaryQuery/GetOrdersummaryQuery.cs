using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Order.GetOrdersummaryQuery;

public record GetOrdersummaryQuery(int OrderId) 
    : IRequest<IResult<IReadOnlyList<GetOrdersummaryQueryResponse>>>;
