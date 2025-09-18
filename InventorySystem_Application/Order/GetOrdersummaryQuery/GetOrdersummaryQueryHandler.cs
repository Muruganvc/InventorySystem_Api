using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Order.GetOrdersummaryQuery;
internal sealed class GetOrdersummaryQueryHandler
    : IRequestHandler<GetOrdersummaryQuery, IResult<IReadOnlyList<GetOrdersummaryQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IRepository<InventorySystem_Domain.ProductCategory> _productCategoryRepository;
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
        IRepository<InventorySystem_Domain.Customer> customerRepository,
        IRepository<InventorySystem_Domain.ProductCategory> productCategoryRepository
    )
    {
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _orderItemRepository = orderItemRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productCategoryRepository = productCategoryRepository;
    }
    public async Task<IResult<IReadOnlyList<GetOrdersummaryQueryResponse>>> Handle(GetOrdersummaryQuery request, CancellationToken cancellationToken)
    {
        var resultList = await _orderItemRepository.Table.AsNoTracking()
                .Join(_productRepository.Table.AsNoTracking(),
                    orderItem => orderItem.ProductId,
                    product => product.ProductId,
                    (orderItem, product) => new { orderItem, product })
                .Join(_productCategoryRepository.Table.AsNoTracking(), 
                    temp => temp.product.ProductCategoryId,
                    productCategory => productCategory.ProductCategoryId,
                    (temp, productCategory) => new { temp.orderItem, temp.product, productCategory })
                .Join(_categoryRepository.Table.AsNoTracking(),
                    temp => temp.productCategory.CategoryId,
                    category => category.CategoryId,
                    (temp, category) => new { temp.orderItem, temp.product, temp.productCategory, category })
                .Join(_companyRepository.Table.AsNoTracking(),
                    temp => temp.category.CompanyId,
                    company => company.CompanyId,
                    (temp, company) => new { temp.orderItem, temp.product, temp.productCategory, temp.category, company })
                .Join(_orderRepository.Table.AsNoTracking().Where(order => order.OrderId == request.OrderId),
                    temp => temp.orderItem.OrderId,
                    order => order.OrderId,
                    (temp, order) => new { temp.orderItem, temp.product, temp.productCategory, temp.category, temp.company, order })
                .Join(_customerRepository.Table.AsNoTracking(),
                    temp => temp.order.CustomerId,
                    customer => customer.CustomerId,
                    (temp, customer) => new GetOrdersummaryQueryResponse(
                        temp.orderItem.ProductId,
                        temp.product.ProductName,
                        temp.orderItem.Quantity,
                        temp.orderItem.UnitPrice,
                        temp.orderItem.DiscountPercent,
                        temp.orderItem.DiscountAmount,
                        temp.orderItem.SubTotal,
                        temp.orderItem.NetTotal,
                        temp.order.OrderDate,
                        temp.order.FinalAmount ?? 0,
                        temp.order.TotalAmount ?? 0,
                        temp.order.BalanceAmount ?? 0,
                        customer.CustomerName,
                        customer.Address,
                        customer.Phone,
                        temp.orderItem.CreatedUser.UserName,
                        temp.orderItem.SerialNo,
                        temp.order.IsGst,
                        temp.orderItem.Meter ?? 0
                    ))
                .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<GetOrdersummaryQueryResponse>>.Success(resultList.OrderBy(a => a.FullProductName).ToList());
    }
}
