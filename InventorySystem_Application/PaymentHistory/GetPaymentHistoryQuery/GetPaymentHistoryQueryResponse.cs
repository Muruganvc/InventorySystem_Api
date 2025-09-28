namespace InventorySystem_Application.PaymentHistory.GetPaymentHistoryQuery;
public record GetPaymentHistoryQueryResponse(string CustomerName,
    decimal FinalAmount,
    decimal AmountPaid,
    decimal BalanceRemainingToPay,
    DateTime PaymentAt,
    string PaymentMethod,
    string TransactionRefNo,
    string UserName);
