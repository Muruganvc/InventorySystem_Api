using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.AddOrRemoveUserRoleCommand;
public record AddOrRemoveUserRoleCommand(int UserId, int RoleId)
    :IRequest<IResult<bool>>;
