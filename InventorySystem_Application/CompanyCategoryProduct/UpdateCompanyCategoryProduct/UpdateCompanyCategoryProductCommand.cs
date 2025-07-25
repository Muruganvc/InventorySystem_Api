using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.CompanyCategoryProduct.UpdateCompanyCategoryProduct;

public record UpdateCompanyCategoryProductCommand(int CompanyCategoryProductItemId, 
            string CompanyCategoryProductItemName, 
            int CategoryId, 
            string? Description, 
            bool IsActive):IRequest<IResult<bool>>;

