namespace InventorySystem_Api.Request;
public record CreateCompanyCategoryProductRequest(
    string? CategoryProductName, 
    int CategoryId, string? Description, bool IsActive);
