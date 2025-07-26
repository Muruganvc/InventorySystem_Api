namespace InventorySystem_Application.Dashboard.GetProductQuantityQuery;
public record GetProductQuantityQueryResponse(
    string CompanyName,
    string CategoryName,
    string ProductCategoryName,
    int Quantity
    );
