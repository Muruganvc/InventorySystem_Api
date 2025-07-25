using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Company.CreateCompanyCommand;

internal sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, IResult<int>>
{
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(IRepository<InventorySystem_Domain.Company> companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<int>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var IsExistCompany = await _companyRepository.GetByAsync(a => a.CompanyName == request.Name);
        if (IsExistCompany != null)
            return Result<int>.Failure("Entered company already exists");

        var company = InventorySystem_Domain.Company.Create(request.Name, 1, request.Description);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.Repository<InventorySystem_Domain.Company>().AddAsync(company);
            await _unitOfWork.SaveAsync();
        }, cancellationToken);
        return Result<int>.Success(company.CompanyId);
    }
}
