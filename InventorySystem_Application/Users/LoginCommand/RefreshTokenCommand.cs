using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.LoginCommand;

public record RefreshTokenCommand(string RefreshToken) : 
    IRequest<IResult<LoginCommandResponse>>;
