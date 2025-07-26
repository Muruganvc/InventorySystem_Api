namespace InventorySystem_Application.Category.GetCategoriesByCompanyQuery;
public record GetCategoriesByCompanyQueryResponse(int CategoryId, string CategoryName,
int CompanyId, string CompanyName, string? Description, bool IsActive,
uint RowVersion, string CreatedBy, DateTime CreatedAt);
