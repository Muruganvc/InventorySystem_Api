namespace InventorySystem_Application.Category.GetCategoriesQuery;
public record GetCategoriesQueryResponse(int CategoryId, string CategoryName,
    int CompanyId, string CompanyName, string? Description, bool IsActive,
    uint RowVersion, string UserName);
