namespace InventorySystem_Api.Request;
public record CreateProductRequest(string ProductName, int ProductCategoryId,
    string? Description, decimal Mrp, decimal SalesPrice, int Quantity,
    decimal LandingPrice,int Meter,
    bool IsActive = false);
