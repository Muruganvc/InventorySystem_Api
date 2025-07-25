using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Category.CreateCategoryCommand;
public record CreateCategoryCommand(string CategoryName, int CompanyId,string? Description, bool IsActive) : IRequest<IResult<int>>;
