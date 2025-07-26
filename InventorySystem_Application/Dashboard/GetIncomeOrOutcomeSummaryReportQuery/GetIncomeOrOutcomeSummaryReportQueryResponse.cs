namespace InventorySystem_Application.Dashboard.GetIncomeOrOutcomeSummaryReportQuery;
public record GetIncomeOrOutcomeSummaryReportQueryResponse(int ProductId,
    string ProductName,
    decimal SalesPrice,
    decimal LandingPrice,
    decimal MRP,
    decimal AvgUnitPrice,
    int TotalQuantity,
    decimal TotalGainedAmount);
