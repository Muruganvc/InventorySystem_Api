using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.LogoutCommand;

public record LogoutCommand(int UserId):
    IRequest<IResult<bool>>;
