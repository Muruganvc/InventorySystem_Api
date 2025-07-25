using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Product.GetProductQuery;

internal sealed class GetProductQueryHandler
    : IRequestHandler<GetProductQuery, IResult<GetProductQueryResponse>>
{

    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    public GetProductQueryHandler(IRepository<InventorySystem_Domain.Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IResult<GetProductQueryResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var result = await _productRepository.Table
                .AsNoTracking()
                .Where(p => p.ProductId == request.ProductId)
                .Select(p => new GetProductQueryResponse(
                         p.ProductId,
                         p.ProductName,
                         p.ProductCategory!.CategoryId,
                         p.ProductCategory.ProductCategoryName,
                         p.Description,
                         p.MRP,
                         p.SalesPrice,
                         p.LandingPrice,
                         p.Quantity,
                         p.IsActive,
                         p.CreatedByUser.UserName
     ))
     .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
            return Result<GetProductQueryResponse>.Failure("Product not found.");

        return Result<GetProductQueryResponse>.Success(result);
    }
}
