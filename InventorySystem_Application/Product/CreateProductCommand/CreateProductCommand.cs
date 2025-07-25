using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Product.CreateProductCommand;
public record CreateProductCommand(string ProductName, int ProductCategoryId,
    string? Description, decimal Mrp, decimal SalesPrice, int Quantity, 
    decimal LandingPrice,
    bool IsActive = false
) : IRequest<IResult<int>>;
