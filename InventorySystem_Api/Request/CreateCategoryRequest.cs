namespace InventorySystem_Api.Request;

public record CreateCategoryRequest(string CategoryName, int CompanyId, string? Description, bool IsActive);
 