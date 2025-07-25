namespace InventorySystem_Api.Request;
public record CreateCompanyRequest(string CompanyName, string Description, bool IsActive);
