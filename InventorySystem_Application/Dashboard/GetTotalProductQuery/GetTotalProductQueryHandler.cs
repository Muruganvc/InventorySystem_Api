using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using InventorySystem_Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Dashboard.GetTotalProductQuery;
internal sealed class GetTotalProductQueryHandler
    : IRequestHandler<GetTotalProductQuery, IResult<GetTotalProductQueryResponse>>
{
    private readonly IRepository<OrderItem> _orderItemRepository;
    private readonly IRepository<InventorySystem_Domain.Order> _orderRepository;
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    public GetTotalProductQueryHandler(IRepository<OrderItem> orderItemRepository, IRepository<InventorySystem_Domain.Product> productRepository,
        IRepository<InventorySystem_Domain.Order> orderRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<InventorySystem_Domain.Category> categoryRepository, IRepository<InventorySystem_Domain.Company> companyRepository)
    {
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _productCategoryRepository = productCategoryRepository;
        _categoryRepository = categoryRepository;
        _companyRepository = companyRepository;
    }
    public async Task<IResult<GetTotalProductQueryResponse>> Handle(GetTotalProductQuery request, CancellationToken cancellationToken)
    {
        // Calculate total quantity and net total from order items
        var orderItems = await _orderItemRepository.Table
            .Join(_productRepository.Table,
                orderItem => orderItem.ProductId,
                product => product.ProductId,
                (orderItem, product) => new
                {
                    orderItem.Quantity,
                    NetTotal = (product.MRP - orderItem.UnitPrice) * orderItem.Quantity
                })
            .ToListAsync(cancellationToken);

        var totalQuantity = orderItems.Sum(x => x.Quantity);

        // Calculate total revenue and outstanding balance from orders
        var orders = await _orderRepository.Table
            .Select(order => new
            {
                order.TotalAmount,
                order.BalanceAmount
            })
            .ToListAsync(cancellationToken);

        var totalRevenue = orders.Sum(x => x.TotalAmount ?? 0);
        var outstandingAmount = orders.Sum(x => x.BalanceAmount ?? 0);

        // Calculate company-wise sales summary
        var orderData = await _orderItemRepository.Table
    .Join(_productRepository.Table,
        ori => ori.ProductId,
        prod => prod.ProductId,
        (ori, prod) => new
        {
            ori.ProductId,
            ori.Quantity,
            ori.UnitPrice,
            ProductMRP = prod.MRP,
            ori,
            prod
        })
    .Join(_productCategoryRepository.Table,
        op => op.prod.ProductCategoryId,
        pCat => pCat.ProductCategoryId,
        (op, pCat) => new { op.ori, op.prod, pCat })
    .Join(_categoryRepository.Table,
        opp => opp.pCat.CategoryId,
        cat => cat.CategoryId,
        (opp, cat) => new { opp.ori, opp.prod, opp.pCat, cat })
    .Join(_companyRepository.Table,
        oppc => oppc.cat.CompanyId,
        comp => comp.CompanyId,
        (oppc, comp) => new
        {
            OrderItem = oppc.ori,
            Product = oppc.prod,
            ProductCategory = oppc.pCat,
            Category = oppc.cat,
            Company = comp
        })
    .ToListAsync(cancellationToken);

        var companyWiseTotals = orderData
            .GroupBy(x => new
            {
                x.Company.CompanyId,
                x.Company.CompanyName
            })
            .OrderBy(g => g.Key.CompanyName)
            .Select(g => new GetCompanyWiseSalesResponse(
                g.Key.CompanyId,
                g.Key.CompanyName,
                g.Sum(x => x.OrderItem.Quantity),
                g.Sum(x => x.OrderItem.NetTotal)
            ))
            .ToList();

        // Construct final response
        var response = new GetTotalProductQueryResponse(
            TotalQuantity: totalQuantity,
            TotalNetAmount: totalRevenue,
            BalanceAmount: outstandingAmount,
            CompanyWiseSales: companyWiseTotals
        );

        return Result<GetTotalProductQueryResponse>.Success(response);
    }
}
