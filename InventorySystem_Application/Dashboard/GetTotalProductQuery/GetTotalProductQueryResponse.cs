namespace InventorySystem_Application.Dashboard.GetTotalProductQuery;
public record GetCompanyWiseSalesResponse(
       int CompanyId,
       string CompanyName,
       decimal TotalQuantity,
       decimal TotalNetTotal
   );
public record GetTotalProductQueryResponse (decimal TotalQuantity,
    decimal TotalNetAmount,
    decimal BalanceAmount,
    List<GetCompanyWiseSalesResponse> CompanyWiseSales);