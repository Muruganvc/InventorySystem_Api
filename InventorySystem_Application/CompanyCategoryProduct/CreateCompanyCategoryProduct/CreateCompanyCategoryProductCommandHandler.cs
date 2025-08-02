using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.CompanyCategoryProduct.CreateCompanyCategoryProduct;

internal sealed class CreateCompanyCategoryProductCommandHandler : IRequestHandler<CreateCompanyCategoryProductCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IUserInfo _userInfo;
    public CreateCompanyCategoryProductCommandHandler(IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.Category> categoryRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
        _productCategoryRepository = productCategoryRepository;
        _userInfo = userInfo;
    }

    public async Task<IResult<int>> Handle(CreateCompanyCategoryProductCommand request, CancellationToken cancellationToken)
    {
        var isExistsCategory = await _categoryRepository.Table
            .Include(c => c.Company).FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);
        if (isExistsCategory == null)
            return Result<int>.Failure("Selected category not found.");

        var normalizedName = request.ProductCategoryName?.Replace(" ", string.Empty).ToLower();

        var isExistsItem = await _productCategoryRepository.GetByAsync(a =>
              a.CategoryId == request.CategoryId &&
              a.ProductCategoryName == request.ProductCategoryName);

        if (isExistsItem != null)
        {
            return Result<int>.Failure($"A Product Item named '{request.ProductCategoryName}' already exists for the selected category.");
        }
        var productItem = ProductCategory.Create(request.ProductCategoryName, request.CategoryId,
            _userInfo.UserId, request.Description, request.IsActive);

        var productItemId = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _productCategoryRepository.AddAsync(productItem);
            await _unitOfWork.SaveAsync();
            return productItem.ProductCategoryId;
        }, cancellationToken);

        return Result<int>.Success(productItemId);
    }
}
