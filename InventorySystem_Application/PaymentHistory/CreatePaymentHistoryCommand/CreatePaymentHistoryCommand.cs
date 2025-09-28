using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.PaymentHistory.CreatePaymentHistoryCommand;
public record CreatePaymentHistoryCommand(int CustomerId, int OrderId, string PaymentMethod,
    decimal AmountPaid, decimal BalanceRemainingToPay, string TransactionRefNo)
    : IRequest<IResult<bool>>;
