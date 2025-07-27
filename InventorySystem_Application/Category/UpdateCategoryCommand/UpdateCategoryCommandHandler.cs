using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Category.UpdateCategoryCommand;

internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IUserInfo _userInfo;
    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IRepository<InventorySystem_Domain.Company> companyRepository,
        IRepository<InventorySystem_Domain.Category> categoryRepository, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _userInfo = userInfo;
    }
    public async Task<IResult<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByAsync(c => c.CategoryId == request.CategoryId);
        if (category is null)
            return Result<bool>.Failure("Selected category not found.");

        if (category.RowVersion != request.RowVersion)
            return Result<bool>.Failure("The category has been modified by another user. Please reload and try again.");

        var company = await _companyRepository.GetByAsync(c => c.CompanyId == request.CompanyId);
        if (company is null)
            return Result<bool>.Failure("Selected company not found.");

        category.Update(
            name: request.Name,
            companyId: request.CompanyId,
            description: request.Description,
            isActive: request.IsActive,
            modifiedBy: _userInfo.UserId
        );

        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }
}