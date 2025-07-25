using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.ForgetPasswordCommand;
public record ForgetPasswordCommand(int UserId, string MobileNo, string Password)
    :IRequest<IResult<bool>>;


