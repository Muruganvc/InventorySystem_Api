using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Company.UpdateCompnayCommand;
internal sealed class UpdateCompnayCommandHandler : IRequestHandler<UpdateCompnayCommand, IResult<bool>>
{
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;

    public UpdateCompnayCommandHandler(IRepository<InventorySystem_Domain.Company> companyRepository, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }
    public async Task<IResult<bool>> Handle(UpdateCompnayCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByAsync(c => c.CompanyId == request.CompanyId);

        if (company is null)
            return Result<bool>.Failure("Selected company not found.");

        if (company.RowVersion != request.RowVersion)
            return Result<bool>.Failure("The company has been modified by another user. Please reload and try again.");

        company.Update(request.CompanyName,request.IsActive, request.Description, modifiedBy: _userInfo.UserId);

        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }

}
