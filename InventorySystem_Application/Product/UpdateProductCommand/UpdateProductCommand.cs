using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Product.UpdateProductCommand;

public record UpdateProductCommand(int ProductId, string ProductName, int ProductCategoryId,
    string? Description, decimal Mrp, decimal SalesPrice, int Quantity,
    decimal LandingPrice,
    bool IsActive,
    uint RowVersion,
    int Meter
) : IRequest<IResult<bool>>;
