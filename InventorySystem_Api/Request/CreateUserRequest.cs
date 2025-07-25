namespace InventorySystem_Api.Request;
public record CreateUserRequest(string FirstName, string? LastName,
string UserName, string Password, string Email, string MobileNo);

