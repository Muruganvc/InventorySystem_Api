using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.GetRoleByUserQuery;
public record GetRoleByUserQuery(int UserId)
    : IRequest<IResult<IReadOnlyList<GetRoleByUserQueryResponse>>>;
