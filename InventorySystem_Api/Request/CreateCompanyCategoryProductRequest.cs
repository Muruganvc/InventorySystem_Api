namespace InventorySystem_Api.Request;
public record CreateCompanyCategoryProductRequest(
    string categoryProductName, 
    int CategoryId, string? Description, bool IsActive);
