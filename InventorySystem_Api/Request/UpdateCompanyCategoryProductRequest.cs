namespace InventorySystem_Api.Request;

public record UpdateCompanyCategoryProductRequest(int CompanyCategoryProductItemId,
            string CompanyCategoryProductItemName,
            int CategoryId,
            string? Description,
            bool IsActive, uint RowVersion);