namespace InventorySystem_Application.Users.GetUserQuery;

public record GetUserQueryResponse(int UserId,
    string FirstName,
    string? LastName,
    string UserName,
    string? Email,
    bool IsActive,
    string MobileNo,
    string ProfileImageBase64
    );