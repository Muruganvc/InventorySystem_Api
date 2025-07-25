using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Product.UpdateProductQuantityCommand;
public record UpdateProductQuantityCommand(int ProductId, int Quantity,uint RowVersion)
    :IRequest<IResult<bool>>;
