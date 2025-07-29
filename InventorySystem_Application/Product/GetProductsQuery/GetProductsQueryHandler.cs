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
        var result = await _productRepository.Table
            .AsNoTracking()
            .Include(p => p.ProductCategory)
                .ThenInclude(pc => pc.Category)
                    .ThenInclude(c => c.Company)
            .Where(p => isProductRequest ||
                (p.IsActive &&
                p.ProductCategory.IsActive &&
                p.ProductCategory.Category.IsActive &&
                p.ProductCategory.Category.Company.IsActive)
            ).Select(p => new GetProductsQueryResponse(p.ProductId,
                    p.ProductName,
                    p.ProductCategoryId,
                    p.ProductCategory.ProductCategoryName,
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
                    p.RowVersion
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetProductsQueryResponse>>.Success(result);
    }

}
