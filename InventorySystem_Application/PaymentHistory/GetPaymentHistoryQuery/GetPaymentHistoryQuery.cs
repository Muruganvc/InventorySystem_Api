using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.PaymentHistory.GetPaymentHistoryQuery;

public record GetPaymentHistoryQuery(int OrderId)
     : IRequest<IResult<IReadOnlyList<GetPaymentHistoryQueryResponse>>>;


