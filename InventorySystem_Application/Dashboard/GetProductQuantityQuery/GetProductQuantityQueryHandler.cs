using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Dashboard.GetProductQuantityQuery;
internal sealed class GetProductQuantityQueryHandler
    : IRequestHandler<GetProductQuantityQuery, IResult<IReadOnlyList<GetProductQuantityQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    private readonly IRepository<InventorySystem_Domain.ProductCategory> _productCategoryRepository;
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;

    public GetProductQuantityQueryHandler(
        IRepository<InventorySystem_Domain.Category> categoryRepository,
        IRepository<InventorySystem_Domain.Product> productRepository,
        IRepository<InventorySystem_Domain.ProductCategory> productCategoryRepository,
        IRepository<InventorySystem_Domain.Company> companyRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
        _companyRepository = companyRepository;
    }
    public async Task<IResult<IReadOnlyList<GetProductQuantityQueryResponse>>> Handle(GetProductQuantityQuery request, CancellationToken cancellationToken)
    {
        var result = await _companyRepository.Table.AsNoTracking()
                 .Join(_categoryRepository.Table,
                       company => company.CompanyId,
                       category => category.CompanyId,
                       (company, category) => new { company, category })
                 .Join(_productCategoryRepository.Table.AsNoTracking(),
                       x => x.category.CategoryId,
                       productCategory => productCategory.CategoryId,
                       (x, productCategory) => new { x.company, x.category, productCategory })
                 .Join(_productRepository.Table.AsNoTracking(),
                       x => x.productCategory.ProductCategoryId,
                       product => product.ProductCategoryId,
                       (x, product) => new GetProductQuantityQueryResponse(
                           x.company.CompanyName,
                           x.category.CategoryName,
                           x.productCategory.ProductCategoryName,
                           product.Quantity))
                 .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<GetProductQuantityQueryResponse>>.Success(result);
    }
}
