using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using InventorySystem_Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Dashboard.GetIncomeOrOutcomeSummaryReportQuery;
internal sealed class GetIncomeOrOutcomeSummaryReportQueryHandler
    : IRequestHandler<GetIncomeOrOutcomeSummaryReportQuery, IResult<IReadOnlyList<GetIncomeOrOutcomeSummaryReportQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;

    public GetIncomeOrOutcomeSummaryReportQueryHandler(IRepository<InventorySystem_Domain.Product> productRepository, IRepository<OrderItem> orderItemRepository)
    {
        _productRepository = productRepository;
        _orderItemRepository = orderItemRepository;
    }
    public async Task<IResult<IReadOnlyList<GetIncomeOrOutcomeSummaryReportQueryResponse>>> Handle(GetIncomeOrOutcomeSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var result = await _orderItemRepository.Table
             .Where(ord =>
                 !request.FromDate.HasValue || !request.EndDate.HasValue ||
                 (ord.CreatedAt >= request.FromDate && ord.CreatedAt <= request.EndDate)
             )
             .Join(_productRepository.Table,
                 ord => ord.ProductId,
                 p => p.ProductId,
                 (ord, p) => new { Order = ord, Product = p }
             )
             .GroupBy(x => new
             {
                 x.Product.ProductId,
                 x.Product.ProductName,
                 x.Product.SalesPrice,
                 x.Product.LandingPrice,
                 x.Product.MRP
             })
             .Select(g => new GetIncomeOrOutcomeSummaryReportQueryResponse(
                 g.Key.ProductId,
                 g.Key.ProductName,
                 g.Key.SalesPrice,
                 g.Key.LandingPrice,
                 g.Key.MRP,
                 g.Average(x => x.Order.UnitPrice),
                 g.Sum(x => x.Order.Quantity),
                 g.Sum(x => (x.Order.UnitPrice - x.Product.LandingPrice) * x.Order.Quantity)
             ))
             .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetIncomeOrOutcomeSummaryReportQueryResponse>>.Success(result);
    }
}
