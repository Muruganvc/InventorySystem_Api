using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.CompanyCategoryProduct.CreateCompanyCategoryProduct;
public record CreateCompanyCategoryProductCommand(string? ProductCategoryName, int CategoryId,string? Description, bool IsActive)
    :IRequest<IResult<int>>;
