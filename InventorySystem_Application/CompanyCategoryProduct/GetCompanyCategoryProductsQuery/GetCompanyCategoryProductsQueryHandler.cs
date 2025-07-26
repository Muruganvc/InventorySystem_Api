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
    .Include(p => p.Category).ThenInclude(c => c.Company)
    .AsNoTracking()
    .Where(p => request.IsAllActiveCompany || (p.IsActive && p.Category.IsActive))
    .Select(p => new GetCompanyCategoryProductsQueryResponse(
        p.ProductCategoryId,
        p.ProductCategoryName,
        p.Category.CategoryId,
        p.Category.CategoryName,
        p.Category.Company.CompanyId,
        p.Category.Company.CompanyName,
        p.Description,
        p.IsActive,
        p.RowVersion,
        p.CreatedByUser.UserName,
        p.CreatedAt
    ))
    .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<GetCompanyCategoryProductsQueryResponse>>.Success(categoryProducts);
    }
}
