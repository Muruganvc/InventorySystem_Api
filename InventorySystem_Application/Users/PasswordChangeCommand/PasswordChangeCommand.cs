using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.PasswordChangeCommand;
public record PasswordChangeCommand(
    int UserId,
    string CurrentPassword,
    string PasswordHash
) : IRequest<IResult<bool>>
{
    public static DateTime PasswordExpiresAt => DateTime.UtcNow.AddDays(30);
}
