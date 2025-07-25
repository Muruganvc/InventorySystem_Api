using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductsQuery;
internal sealed class GetCompanyCategoryProductsQueryHandler :
    IRequestHandler<GetCompanyCategoryProductsQuery, IResult<IReadOnlyList<GetCompanyCategoryProductsQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.ProductCategory> _productCategoryRepository;

    public GetCompanyCategoryProductsQueryHandler(
        IRepository<InventorySystem_Domain.ProductCategory> productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;
    }
    public async Task<IResult<IReadOnlyList<GetCompanyCategoryProductsQueryResponse>>> Handle(GetCompanyCategoryProductsQuery request, CancellationToken cancellationToken)
    {
        var categoryProducts = await _productCategoryRepository.Table
          .AsNoTracking()
          .Where(c => request.IsAllActiveCompany || (c.IsActive && c.Category.IsActive))
          .Select(p => new GetCompanyCategoryProductsQueryResponse(
              p.ProductCategoryId,
              p.ProductCategoryName,
              p.Category.CategoryId,
              p.Category.CategoryName,
              p.Description,
              p.IsActive,
              p.RowVersion,
              p.CreatedByUser.UserName
          ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetCompanyCategoryProductsQueryResponse>>.Success(categoryProducts);
    }
}
