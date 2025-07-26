using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.CompanyCategoryProduct.UpdateCompanyCategoryProduct;

public record UpdateCompanyCategoryProductCommand(
            int ProductCategoryId,
            string ProductCategoryName,
            int CategoryId,
            string? Description,
            bool IsActive, uint RowVersion) : IRequest<IResult<bool>>;

