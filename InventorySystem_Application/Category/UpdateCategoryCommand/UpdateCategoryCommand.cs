using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Category.UpdateCategoryCommand;

public record UpdateCategoryCommand(int CategoryId, string Name, int CompanyId, string? Description, bool IsActive)
    : IRequest<IResult<bool>>;