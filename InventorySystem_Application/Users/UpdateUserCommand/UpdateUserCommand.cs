using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.UpdateUserCommand;
public record UpdateUserCommand(int UserId,
    string FirstName,
    string LastName,
    string Email,
    byte[]? ImageData,
    string MobileNo) :IRequest<IResult<bool>>;
