namespace InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductsQuery;
public record GetCompanyCategoryProductsQueryResponse(int ProductCategoryId,
    string ProductCategoryName,
    int CategoryId,
    string CategoryName,
    int CompanyId,
    string CompanyName,
    string? Description,
    bool IsActive,
    uint RowVersion,
    string Username,
    DateTime CreatedAt
    );
