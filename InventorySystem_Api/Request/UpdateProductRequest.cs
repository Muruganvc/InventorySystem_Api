namespace InventorySystem_Api.Request;
public record UpdateProductRequest(int ProductId, string ProductName, int ProductCategoryId,
    string? Description, decimal Mrp, decimal SalesPrice, int Quantity,
    decimal LandingPrice,
    bool IsActive,uint RowVersion);
