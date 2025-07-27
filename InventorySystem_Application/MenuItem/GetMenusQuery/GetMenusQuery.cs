using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.MenuItem.GetMenusQuery;
public record GetMenusQuery()
    : IRequest<IResult<IReadOnlyList<GetMenusQueryResponse>>>;

