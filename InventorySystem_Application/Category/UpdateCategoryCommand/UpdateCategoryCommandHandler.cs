using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Category.UpdateCategoryCommand;

internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IRepository<InventorySystem_Domain.Company> companyRepository,
        IRepository<InventorySystem_Domain.Category> categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
    }
    public async Task<IResult<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByAsync(c => c.CategoryId == request.CategoryId);
        if (category is null)
            return Result<bool>.Failure("Selected category not found.");

        var company = await _companyRepository.GetByAsync(c => c.CompanyId == request.CompanyId);
        if (company is null)
            return Result<bool>.Failure("Selected company not found.");

        category.Update(
            name: request.Name,
            companyId: request.CompanyId,
            description: request.Description,
            isActive: request.IsActive,
            modifiedBy: 1
        );

        var isSuccess = false;
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            isSuccess = await _unitOfWork.SaveAsync() > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }
}