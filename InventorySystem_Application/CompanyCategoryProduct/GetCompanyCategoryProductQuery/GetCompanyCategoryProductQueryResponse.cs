namespace InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductQuery;
public record GetCompanyCategoryProductQueryResponse(int CompanyCategoryProductId, string CompanyCategoryProductName,
    int CategoryId, string CategoryName, string? Description, bool IsActve, uint RowVersion, string UserName);
 
