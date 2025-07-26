namespace InventorySystem_Api.Request;

public record UpdateCompanyCategoryProductRequest(string ProductCategoryName,
            int CategoryId,
            string? Description,
            bool IsActive, uint RowVersion);