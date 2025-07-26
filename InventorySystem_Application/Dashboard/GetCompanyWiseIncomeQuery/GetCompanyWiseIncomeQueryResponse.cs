namespace InventorySystem_Application.Dashboard.GetCompanyWiseIncomeQuery;
public record GetCompanyWiseIncomeQueryResponse(
    string CompanyName,
    string CategoryName,
    string ProductCategoryName,
    int TotalQuantity,
    decimal Income);
