namespace InventorySystem_Api.Request;
public record CreatePaymentHistoryRequest(int CustomerId, int OrderId, string PaymentMethod,
    decimal AmountPaid, decimal BalanceRemainingToPay, string TransactionRefNo);
 