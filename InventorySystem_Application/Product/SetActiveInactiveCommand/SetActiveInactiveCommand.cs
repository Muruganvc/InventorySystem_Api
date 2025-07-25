using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Product.SetActiveInactiveCommand;
public record SetActiveInactiveCommand(int ProductId,uint RowVersion)
    :IRequest<IResult<bool>>;