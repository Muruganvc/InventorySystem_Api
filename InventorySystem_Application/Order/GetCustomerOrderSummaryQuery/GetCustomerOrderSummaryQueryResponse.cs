namespace InventorySystem_Application.Order.GetCustomerOrderSummaryQuery;
public record GetCustomerOrderSummaryQueryResponse(int OrderId,
    string CustomerName,
    string Phone,
    string Address,
    DateTime OrderDate,
    decimal TotalAmount,
    decimal FinalAmount,
    decimal BalanceAmount,
    int CustomerId
   );