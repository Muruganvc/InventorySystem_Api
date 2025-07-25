using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.LoginCommand;
public record LoginCommand(string UserName, string Password)
    : IRequest<IResult<LoginCommandResponse>>;
