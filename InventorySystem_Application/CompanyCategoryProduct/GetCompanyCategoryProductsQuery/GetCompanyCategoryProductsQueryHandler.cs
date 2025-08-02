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
        var query = _productCategoryRepository.Table.AsNoTracking();

        // Apply filters early
        if (!request.IsAllActiveCompany)
        {
            query = query.Where(p =>
                p.IsActive &&
                p.Category.IsActive &&
                p.Category.Company.IsActive);
        }

        // Project before materializing to avoid unnecessary JOIN loading
        var categoryProducts = await query
            .Select(p => new GetCompanyCategoryProductsQueryResponse(
                p.ProductCategoryId,
                p.ProductCategoryName ?? string.Empty,
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
