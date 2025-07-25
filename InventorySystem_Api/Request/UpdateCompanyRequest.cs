namespace InventorySystem_Api.Request;

public record UpdateCompanyRequest(string CompanyName, string? Description, bool IsActive, uint RowVersion);
