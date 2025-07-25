namespace InventorySystem_Api.Request;
public record UpdateCategoryRequest(int CategoryId, string CategoryName, int CompanyId, string? Description, bool IsActive, uint RowVersion);