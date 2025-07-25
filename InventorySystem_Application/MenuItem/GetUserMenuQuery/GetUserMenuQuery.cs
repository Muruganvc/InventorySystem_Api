using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.MenuItem.GetUserMenuQuery;
public record GetUserMenuQuery(int UserId): 
    IRequest<IResult<IReadOnlyList<GetMenuItemQueryResponse>>>;
