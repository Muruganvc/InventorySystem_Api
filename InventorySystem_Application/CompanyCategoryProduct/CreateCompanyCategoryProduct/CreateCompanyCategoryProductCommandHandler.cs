using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.CompanyCategoryProduct.CreateCompanyCategoryProduct;

internal sealed class CreateCompanyCategoryProductCommandHandler : IRequestHandler<CreateCompanyCategoryProductCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    public CreateCompanyCategoryProductCommandHandler(IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.Category> categoryRepository,
        IRepository<ProductCategory> productCategoryRepository)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
        _productCategoryRepository = productCategoryRepository;
    }

    public async Task<IResult<int>> Handle(CreateCompanyCategoryProductCommand request, CancellationToken cancellationToken)
    {

        var isExistscompany = await _categoryRepository.GetByAsync(a => a.CategoryId == request.CategoryId);
        if (isExistscompany == null)
            return Result<int>.Failure("Selected category not found.");

        var isExistsItem = await _productCategoryRepository.GetByAsync(a => a.CategoryId == request.CategoryId
            && a.ProductCategoryName == request.CompanyCategoryProductItemName);
        if (isExistsItem != null)
            return Result<int>.Failure($"A Product Item named '{request.CompanyCategoryProductItemName}' already exists for the selected category.");

        var productItem = ProductCategory.Create(request.CompanyCategoryProductItemName, request.CategoryId,
            1, request.Description, request.IsActive);

        var productItemId = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _productCategoryRepository.AddAsync(productItem);
            await _unitOfWork.SaveAsync();
            return productItem.ProductCategoryId;
        }, cancellationToken);

        return Result<int>.Success(productItemId);
    }
}
