namespace InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductsQuery;
public record GetCompanyCategoryProductsQueryResponse(int CompanyCategoryProductId, string CompanyCategoryProductName,
    int CategoryId, string CategoryName, string? Description, bool IsActve, uint RowVersion, string UserName);
