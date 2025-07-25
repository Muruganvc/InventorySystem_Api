using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.CreateUserCommand;
public record CreateUserCommand(string FirstName, string? LastName,
    string UserName,string Password, string Email, string MobileNo)
    :IRequest<IResult<int>>
{
    public DateTime PasswordExpiresAt => DateTime.UtcNow.AddDays(30);
}

