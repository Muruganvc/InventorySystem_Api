using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Product.GetProductsQuery;

internal sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, IResult<IReadOnlyList<GetProductsQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    public GetProductsQueryHandler(IRepository<InventorySystem_Domain.Product> productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<IResult<IReadOnlyList<GetProductsQueryResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        bool isProductRequest = request.type.Equals("product", StringComparison.OrdinalIgnoreCase);

        var query = _productRepository.Table.AsNoTracking();

        // Filter early (important for SQL performance)
        if (!isProductRequest)
        {
            query = query.Where(p =>
                p.IsActive &&
                p.ProductCategory.IsActive &&
                p.ProductCategory.Category.IsActive &&
                p.ProductCategory.Category.Company.IsActive);
        }

        // Project before materializing — no need for full entity loading
        var result = await query
            .Select(p => new GetProductsQueryResponse(
                p.ProductId,
                p.ProductName,
                p.ProductCategoryId,
                p.ProductCategory.ProductCategoryName!,
                p.ProductCategory.Category.CategoryId,
                p.ProductCategory.Category.CategoryName,
                p.ProductCategory.Category.Company.CompanyId,
                p.ProductCategory.Category.Company.CompanyName,
                p.Description,
                p.MRP,
                p.SalesPrice,
                p.LandingPrice,
                p.Quantity,
                p.IsActive,
                p.CreatedByUser.UserName,
                p.RowVersion,
                p.Meter ?? 0
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetProductsQueryResponse>>.Success(result.OrderBy(a => a.ProductName).ToList());
    }
}