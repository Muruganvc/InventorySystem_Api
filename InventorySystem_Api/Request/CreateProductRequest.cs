namespace InventorySystem_Api.Request;
public record CreateProductRequest(string ProductName, int ProductCategoryId,
    string? Description, decimal Mrp, decimal SalesPrice, int Quantity,
    decimal LandingPrice,string Length,
    bool IsActive = false);
