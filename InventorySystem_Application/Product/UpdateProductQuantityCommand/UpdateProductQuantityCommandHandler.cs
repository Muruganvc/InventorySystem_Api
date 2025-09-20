using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Product.UpdateProductQuantityCommand;
internal sealed class UpdateProductQuantityCommandHandler
    : IRequestHandler<UpdateProductQuantityCommand, IResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.Product> _productRepository;
    private readonly IUserInfo _userInfo;
    public UpdateProductQuantityCommandHandler(IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.Product> productRepository,
        IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _userInfo = userInfo;
    }
    public async Task<IResult<bool>> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByAsync(a => a.ProductId == request.ProductId);
        if (product == null)
            return Result<bool>.Failure("Selected product not found");

        if (product.RowVersion != request.RowVersion)
            return Result<bool>.Failure("The product item has been modified by another user. Please reload and try again.");

        if ((request.Quantity > 0 && request.Meter > 0) ||
            (request.Quantity <= 0 && request.Meter <= 0))
        {
            return Result<bool>.Failure("Please enter either Quantity or Meter, not both or none.");
        }

        product.QuantityUpdate(request.Quantity, request.Meter, _userInfo.UserId);
        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }
}
