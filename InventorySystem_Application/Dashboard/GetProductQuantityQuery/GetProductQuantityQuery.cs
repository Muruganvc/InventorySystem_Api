using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Dashboard.GetProductQuantityQuery;
public record GetProductQuantityQuery()
    :IRequest<IResult<IReadOnlyList<GetProductQuantityQueryResponse>>>;