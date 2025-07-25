namespace InventorySystem_Api.Request;

public record UpdateCompanyRequest(string Name, string? Description, bool IsActive, uint RowVersion);
