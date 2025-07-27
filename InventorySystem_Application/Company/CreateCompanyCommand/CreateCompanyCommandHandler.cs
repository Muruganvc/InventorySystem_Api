using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Company.CreateCompanyCommand;

internal sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, IResult<int>>
{
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;
    public CreateCompanyCommandHandler(IRepository<InventorySystem_Domain.Company> companyRepository, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }
    public async Task<IResult<int>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var IsExistCompany = await _companyRepository.GetByAsync(a => a.CompanyName == request.Name);
        if (IsExistCompany != null)
            return Result<int>.Failure("Entered company already exists");

        var company = InventorySystem_Domain.Company.Create(request.Name, _userInfo.UserId, request.IsActive, request.Description);

        var companyId = await _unitOfWork.ExecuteInTransactionAsync<int>(async () =>
        {
            await _companyRepository.AddAsync(company);
            await _unitOfWork.SaveAsync();
            return company.CompanyId;
        }, cancellationToken);

        return Result<int>.Success(companyId);
    }
}
