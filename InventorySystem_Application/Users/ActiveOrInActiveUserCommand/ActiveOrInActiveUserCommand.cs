using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.ActiveOrInActiveUserCommand;

public record ActiveOrInActiveUserCommand(int UserId,bool IsActive)
    :IRequest<IResult<bool>>;

