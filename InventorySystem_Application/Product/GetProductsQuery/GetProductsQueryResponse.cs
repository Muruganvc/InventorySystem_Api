namespace InventorySystem_Application.Product.GetProductsQuery;

public record GetProductsQueryResponse(int ProductId,
    string ProductName,
    int ProductCategoryId,
    string ProductCategoryName,
    string? Description,
    decimal MRP,
    decimal SalesPrice,
    decimal LandingPrice,
    int Quantity,
    bool IsActive,
    string UserName);
