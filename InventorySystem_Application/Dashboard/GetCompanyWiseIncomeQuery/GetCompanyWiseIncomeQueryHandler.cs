using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Dashboard.GetCompanyWiseIncomeQuery;

internal sealed class GetCompanyWiseIncomeQueryHandler
    : IRequestHandler<GetCompanyWiseIncomeQuery, IResult<IReadOnlyList<GetCompanyWiseIncomeQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    public GetCompanyWiseIncomeQueryHandler(IRepository<InventorySystem_Domain.Category> categoryRepository, IRepository<InventorySystem_Domain.Product> productRepository, IRepository<OrderItem> orderItemRepository, IRepository<InventorySystem_Domain.ProductCategory> productCategoryRepository, IRepository<InventorySystem_Domain.Company> companyRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _orderItemRepository = orderItemRepository;
        _productCategoryRepository = productCategoryRepository;
        _companyRepository = companyRepository;
    }
    public async Task<IResult<IReadOnlyList<GetCompanyWiseIncomeQueryResponse>>> Handle(GetCompanyWiseIncomeQuery request, CancellationToken cancellationToken)
    {
        var result = await _orderItemRepository.Table
                 .Where(ord =>
                 ord.CreatedAt.Month == DateTime.Now.AddMonths(-1).Month &&
                 ord.CreatedAt.Year == DateTime.Now.Year)
         .Join(_productRepository.Table,
                 ord => ord.ProductId,
                 pro => pro.ProductId,
                 (ord, pro) => new { ord, pro })
         .Join(_categoryRepository.Table,
                 op => op.pro.ProductCategoryId,
                 cat => cat.CategoryId,
                 (op, cat) => new { op.ord, op.pro, cat })
         .Join(_productCategoryRepository.Table,
                 opc => opc.pro.ProductCategoryId,
                 pcat => pcat.ProductCategoryId,
                 (opc, pcat) => new { opc.ord, opc.pro, opc.cat, pcat })
         .Join(_companyRepository.Table,
                 oppc => oppc.cat.CompanyId,
                 com => com.CompanyId,
                 (oppc, com) => new
                 {
                     com.CompanyName,
                     oppc.cat.CategoryName,
                     oppc.pcat.ProductCategoryName,
                     oppc.ord.Quantity,
                     Income = (oppc.pro.MRP - oppc.ord.UnitPrice) * oppc.ord.Quantity
                 })
         .GroupBy(x => new
         {
             x.CompanyName,
             x.CategoryName,
             x.ProductCategoryName
         })
         .Select(g => new GetCompanyWiseIncomeQueryResponse(
                 g.Key.CompanyName,
                 g.Key.CategoryName,
                 g.Key.ProductCategoryName,
                 g.Sum(x => x.Quantity),
                 g.Sum(x => x.Income)
         )).OrderBy(x => x.CompanyName)
         .ThenBy(x => x.CategoryName)
         .ThenBy(x => x.ProductCategoryName)
         .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<GetCompanyWiseIncomeQueryResponse>>.Success(result);
    }
}
