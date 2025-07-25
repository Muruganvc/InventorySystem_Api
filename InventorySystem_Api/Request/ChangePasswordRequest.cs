namespace InventorySystem_Api.Request;
public record ChangePasswordRequest(string CurrentPassword,string PasswordHash);
