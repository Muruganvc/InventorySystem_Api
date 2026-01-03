namespace InventorySystem_Application.PaymentHistory.GetPaymentHistoryQuery;
public record GetPaymentHistoryQueryResponse(string CustomerName,
    decimal FinalAmount,
    decimal AmountPaid,
    decimal BalanceRemainingToPay,
    DateTime OrderDate,
    string PaymentMethod,
    string TransactionRefNo,
    string UserName);
