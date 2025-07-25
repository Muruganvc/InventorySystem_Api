namespace InventorySystem_Api.Request;
public record CreateCompanyCategoryProductRequest(string CompanyCategoryProductItemName, int CategoryId, string? Description, bool IsActive);
