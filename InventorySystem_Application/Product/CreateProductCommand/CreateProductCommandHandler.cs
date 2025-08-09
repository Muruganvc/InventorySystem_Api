using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Product.CreateProductCommand;

internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, IResult<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    private readonly IUserInfo _userInfo;
    public CreateProductCommandHandler(IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.Product> productRepository,
        IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _userInfo = userInfo;
    }
    public async Task<IResult<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var isExistProduct = await _productRepository.GetByAsync(a => a.ProductName == request.ProductName
        && a.ProductCategoryId == request.ProductCategoryId);
        if (isExistProduct is not null)
            return Result<int>.Failure("Selected product name already exists.");

        var product = InventorySystem_Domain.Product.Create(request.ProductName, request.ProductCategoryId, request.Description,
            request.Mrp, request.SalesPrice, request.Quantity, request.LandingPrice, _userInfo.UserId, request.IsActive);

        var productIId = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveAsync();
            return product.ProductId;
        }, cancellationToken);

        return Result<int>.Success(productIId);
    }
}