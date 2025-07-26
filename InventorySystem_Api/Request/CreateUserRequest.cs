namespace InventorySystem_Api.Request;
public record CreateUserRequest(string FirstName, string? LastName,int role,
string UserName, string Email, string MobileNo);

