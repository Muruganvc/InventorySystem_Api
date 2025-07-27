using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Category.CreateCategoryCommand;
internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IUserInfo _userInfo;
    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.Company> companyRepository, 
        IRepository<InventorySystem_Domain.Category> categoryRepository,
        IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _userInfo = userInfo;
    }
    public async Task<IResult<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var isExistscompany = await _companyRepository.GetByAsync(a => a.CompanyId == request.CompanyId);
        if (isExistscompany == null)
            return Result<int>.Failure("Selected company not found.");

        var isExistsCategory = await _categoryRepository.GetByAsync(a => a.CompanyId == request.CompanyId && a.CategoryName == request.CategoryName);
        if (isExistsCategory != null)
            return Result<int>.Failure($"A category named '{request.CategoryName}' already exists for the selected company.");

        var category = InventorySystem_Domain.Category.Create(request.CategoryName, request.CompanyId, _userInfo.UserId, request.Description, request.IsActive);

        var categoryId = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();
            return category.CategoryId;
        }, cancellationToken);

        return Result<int>.Success(categoryId);
    }
}
