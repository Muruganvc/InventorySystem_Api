using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Order.GetOrdersummaryQuery;
internal sealed class GetOrdersummaryQueryHandler
    : IRequestHandler<GetOrdersummaryQuery, IResult<IReadOnlyList<GetOrdersummaryQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    private readonly IRepository<InventorySystem_Domain.OrderItem> _orderItemRepository;
    private readonly IRepository<InventorySystem_Domain.Order> _orderRepository;
    private readonly IRepository<InventorySystem_Domain.Customer> _customerRepository;
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;

    public GetOrdersummaryQueryHandler(
        IRepository<InventorySystem_Domain.Company> companyRepository,
        IRepository<InventorySystem_Domain.Category> categoryRepository,
        IRepository<InventorySystem_Domain.Product> productRepository,
        IRepository<InventorySystem_Domain.OrderItem> orderItemRepository,
        IRepository<InventorySystem_Domain.Order> orderRepository,
        IRepository<InventorySystem_Domain.Customer> customerRepository
    )
    {
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _orderItemRepository = orderItemRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }
    public async Task<IResult<IReadOnlyList<GetOrdersummaryQueryResponse>>> Handle(GetOrdersummaryQuery request, CancellationToken cancellationToken)
    {
        var resultList = await _orderItemRepository.Table.AsNoTracking()
            .Join(_productRepository.Table.AsNoTracking(),
                odIm => odIm.ProductId,
                pro => pro.ProductId,
                (odIm, pro) => new { odIm, pro })
            .Join(_categoryRepository.Table.AsNoTracking(),
                temp => temp.pro.ProductCategoryId,
                cat => cat.CategoryId,
                (temp, cat) => new { temp.odIm, temp.pro, cat })
            .Join(_companyRepository.Table.AsNoTracking(),
                temp => temp.cat.CompanyId,
                com => com.CompanyId,
                (temp, com) => new { temp.odIm, temp.pro, temp.cat, com })
            .Join(_orderRepository.Table.AsNoTracking().Where(ord => ord.OrderId == request.OrderId),
                temp => temp.odIm.OrderId,
                odr => odr.OrderId,
                (temp, odr) => new { temp.odIm, temp.pro, temp.cat, temp.com, odr })
            .Join(_customerRepository.Table.AsNoTracking(),
                temp => temp.odr.CustomerId,
                cus => cus.CustomerId,
                (temp, cus) => new GetOrdersummaryQueryResponse(
                     temp.odIm.ProductId,
                      $"{temp.com.CompanyName} {temp.cat.CategoryName} {temp.pro.ProductName}",
                     temp.odIm.Quantity,
                     temp.odIm.UnitPrice,
                     temp.odIm.DiscountPercent,
                    temp.odIm.DiscountAmount,
                     temp.odIm.SubTotal,
                     temp.odIm.NetTotal,
                     temp.odr.OrderDate,
                     temp.odr.FinalAmount ?? 0,
                     temp.odr.TotalAmount ?? 0,
                     temp.odr.BalanceAmount ?? 0,
                     cus.CustomerName,
                     cus.Address,
                     cus.Phone,
                     temp.odIm.CreatedUser.UserName,
                     temp.odIm.SerialNo,
                    temp.odr.IsGst
                ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetOrdersummaryQueryResponse>>.Success(resultList.OrderBy(a => a.FullProductName).ToList());
    }

}
