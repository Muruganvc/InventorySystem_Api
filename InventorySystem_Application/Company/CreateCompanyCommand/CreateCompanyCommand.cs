using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Company.CreateCompanyCommand;
public record CreateCompanyCommand(string Name, string? Description, bool IsActive) : IRequest<IResult<int>>;
