using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using InventorySystem_Infrastructure;
using MediatR;

namespace InventorySystem_Application.InventoryCompanyInfo.UpdateInventoryCompanyInfoCommand;
internal sealed class UpdateInventoryCompanyInfoCommandHandler
    : IRequestHandler<UpdateInventoryCompanyInfoCommand, IResult<bool>>
{
    private readonly IRepository<InventorySystem_Domain.InventoryCompanyInfo> _invCompanyInfoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryCompanyInfoCommandHandler(IRepository<InventorySystem_Domain.InventoryCompanyInfo> invCompanyInfoRepository,
        IUnitOfWork unitOfWork)
    {
        _invCompanyInfoRepository = invCompanyInfoRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<bool>> Handle(UpdateInventoryCompanyInfoCommand request, CancellationToken cancellationToken)
    {
        var companyInfo = await _invCompanyInfoRepository.GetByIdAsync(request.InventoryCompanyInfoId);

        if (companyInfo is null)
        {
            return Result<bool>.Failure("Selected company not found.");
        }
        companyInfo.Update(request.InventoryCompanyInfoName, request.Description,
            request.Address, request.MobileNo, request.Email, request.GstNumber, request.BankName, request.BankBranchName,
            request.BankAccountNo, request.BankBranchIFSC, request.ApiVersion, request.UiVersion, request.QrCode);
        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);

    }
}
