namespace InventorySystem_Api.Request;
public record UpdateCategoryRequest(int CategoryId, string Name, int CompanyId, string? Description, bool IsActive, uint RowVersion);