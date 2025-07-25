using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.InventoryCompanyInfo.CreateInventoryCompanyInfoCommand;
internal sealed class CreateInventoryCompanyInfoCommandHandler
    : IRequestHandler<CreateInventoryCompanyInfoCommand, IResult<bool>>
{
    private readonly IRepository<InventorySystem_Domain.InventoryCompanyInfo> _invCompanyInfoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryCompanyInfoCommandHandler(IRepository<InventorySystem_Domain.InventoryCompanyInfo> invCompanyInfoRepository,
        IUnitOfWork unitOfWork)
    {
        _invCompanyInfoRepository = invCompanyInfoRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<bool>> Handle(CreateInventoryCompanyInfoCommand request, CancellationToken cancellationToken)
    {
        var invCompanyInfo = InventorySystem_Domain.InventoryCompanyInfo.Create(request.InventoryCompanyInfoName, request.Description,
            request.Address, request.MobileNo, request.Email, request.GstNumber, request.BankName, request.BankBranchName,
            request.BankAccountNo, request.BankBranchIFSC, request.ApiVersion, request.UiVersion, request.QrCode);

        var invCompanyInfoId = await _unitOfWork.ExecuteInTransactionAsync<int>(async () =>
        {
            await _invCompanyInfoRepository.AddAsync(invCompanyInfo);
            await _unitOfWork.SaveAsync();
            return invCompanyInfo.InventoryCompanyInfoId;
        }, cancellationToken);

        return Result<bool>.Success(invCompanyInfoId > 0);
    }
}