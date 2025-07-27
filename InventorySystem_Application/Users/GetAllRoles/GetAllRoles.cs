using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.GetAllRoles;
public record GetRolesQuery() : IRequest<IResult<IReadOnlyList<GetAllRolesResponse>>>;