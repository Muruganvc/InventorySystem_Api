using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.PaymentHistory.CreatePaymentHistoryCommand;

internal sealed class CreatePaymentHistoryCommandHandler
    : IRequestHandler<CreatePaymentHistoryCommand, IResult<bool>>
{
    private readonly IRepository<InventorySystem_Domain.PaymentHistory> _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;
    public CreatePaymentHistoryCommandHandler(IRepository<InventorySystem_Domain.PaymentHistory> paymentRepository, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }
    public async Task<IResult<bool>> Handle(CreatePaymentHistoryCommand request, CancellationToken cancellationToken)
    {
        var paymentHistory = InventorySystem_Domain.PaymentHistory.Create(request.OrderId, request.CustomerId, request.AmountPaid, request.PaymentMethod,
            request.TransactionRefNo, request.BalanceRemainingToPay, _userInfo.UserId);

        var IsPaymentHistory = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            await _paymentRepository.AddAsync(paymentHistory);
            await _unitOfWork.SaveAsync();
            return paymentHistory.PaymentHistoryId > 0;
        }, cancellationToken);

        return Result<bool>.Success(IsPaymentHistory);
    }
}
