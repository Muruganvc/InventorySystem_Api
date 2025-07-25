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
        var result = await _productRepository.Table
            .AsNoTracking()
            .Select(p => new GetProductsQueryResponse(
                p.ProductId,
                p.ProductName,
                p.ProductCategory.CategoryId,
                p.ProductCategory.ProductCategoryName,
                p.Description,
                p.MRP,
                p.SalesPrice,
                p.LandingPrice,
                p.Quantity,
                p.IsActive,
                p.CreatedByUser.UserName
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetProductsQueryResponse>>.Success(result);
    }
}
