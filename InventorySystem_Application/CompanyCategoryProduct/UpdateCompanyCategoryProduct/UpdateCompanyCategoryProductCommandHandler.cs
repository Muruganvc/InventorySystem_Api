using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.CompanyCategoryProduct.UpdateCompanyCategoryProduct;

internal class UpdateCompanyCategoryProductCommandHandler : IRequestHandler<UpdateCompanyCategoryProductCommand, IResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.ProductCategory> _productCategoryRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    public UpdateCompanyCategoryProductCommandHandler(IUnitOfWork unitOfWork, IRepository<InventorySystem_Domain.ProductCategory> productCategoryRepository, IRepository<InventorySystem_Domain.Category> categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _productCategoryRepository = productCategoryRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IResult<bool>> Handle(UpdateCompanyCategoryProductCommand request, CancellationToken cancellationToken)
    {
        var productCategory = await _productCategoryRepository.GetByAsync(a => a.ProductCategoryId == request.CompanyCategoryProductItemId);
        if (productCategory == null)
            return Result<bool>.Failure("Selected product category not found");

        var category = await _categoryRepository.GetByAsync(a => a.CategoryId == request.CategoryId);
        if (category == null) return Result<bool>.Failure("Selected category not found");

        productCategory.Update(request.CompanyCategoryProductItemName, request.CategoryId, request.Description, request.IsActive, 1);
        var isSuccess = false;
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            isSuccess = await _unitOfWork.SaveAsync() > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }
}
