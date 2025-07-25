using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Company.UpdateCompnayCommand;
public record UpdateCompnayCommand(int Id, string Name, string? Description,bool IsActive, uint Version): IRequest<IResult<bool>>;
