using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductQuery;
internal sealed class GetCompanyCategoryProductQueryHandler
    : IRequestHandler<GetCompanyCategoryProductQuery, IResult<GetCompanyCategoryProductQueryResponse>>
{
    private readonly IRepository<InventorySystem_Domain.ProductCategory> _productCategoryRepository;

    public GetCompanyCategoryProductQueryHandler(
        IRepository<InventorySystem_Domain.ProductCategory> productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }
    public async Task<IResult<GetCompanyCategoryProductQueryResponse>> Handle(
        GetCompanyCategoryProductQuery request,
        CancellationToken cancellationToken)
    {
        var categoryProduct = await _productCategoryRepository.Table
            .AsNoTracking()
            .Where(p => p.ProductCategoryId == request.CompanyCategoryProductId)
            .Select(p => new GetCompanyCategoryProductQueryResponse(
                p.ProductCategoryId,
                p.ProductCategoryName,
                p.Category.CategoryId,
                p.Category.CategoryName,
                p.Description,
                p.IsActive,
                p.RowVersion,
                p.CreatedByUser.UserName
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return categoryProduct is null
            ? Result<GetCompanyCategoryProductQueryResponse>.Failure("Selected company category product not found.")
            : Result<GetCompanyCategoryProductQueryResponse>.Success(categoryProduct);
    }

}
