namespace InventorySystem_Application.Product.GetProductQuery;

public record GetProductQueryResponse(
    int ProductId,
    string ProductName,
    int ProductCategoryId,
    string ProductCategoryName,
    string? Description,
    decimal MRP,
    decimal SalesPrice,
    decimal LandingPrice,
    int Quantity,
    bool IsActive,
    string UserName,
    string Length);
