using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.MenuItem.AddOrRemoveUserMenuItemCommand;

public record AddOrRemoveUserMenuItemCommand(int UserId, int MenuId)
    : IRequest<IResult<bool>>;
