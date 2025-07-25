using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Company.UpdateCompnayCommand;
public record UpdateCompnayCommand(int CompanyId, string CompanyName, string? Description, bool IsActive, uint RowVersion) : IRequest<IResult<bool>>;
