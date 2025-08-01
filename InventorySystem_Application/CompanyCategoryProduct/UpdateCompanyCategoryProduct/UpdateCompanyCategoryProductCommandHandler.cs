﻿using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.CompanyCategoryProduct.UpdateCompanyCategoryProduct;

internal class UpdateCompanyCategoryProductCommandHandler : IRequestHandler<UpdateCompanyCategoryProductCommand, IResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IUserInfo _userInfo;
    public UpdateCompanyCategoryProductCommandHandler(IUnitOfWork unitOfWork, 
        IRepository<ProductCategory> productCategoryRepository, 
        IRepository<InventorySystem_Domain.Category> categoryRepository, IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _productCategoryRepository = productCategoryRepository;
        _categoryRepository = categoryRepository;
        _userInfo = userInfo;
    }

    public async Task<IResult<bool>> Handle(UpdateCompanyCategoryProductCommand request, CancellationToken cancellationToken)
    {
        var productCategory = await _productCategoryRepository.GetByAsync(a => a.ProductCategoryId == request.ProductCategoryId);
        if (productCategory == null)
            return Result<bool>.Failure("Selected product category not found");

        if (productCategory.RowVersion != request.RowVersion)
            return Result<bool>.Failure("The company category product has been modified by another user. Please reload and try again.");

        var category = await _categoryRepository.GetByAsync(a => a.CategoryId == request.CategoryId);
        if (category == null) return Result<bool>.Failure("Selected category not found");

        productCategory.Update(request.ProductCategoryName, request.CategoryId, request.Description, request.IsActive, _userInfo.UserId);
        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }
}
