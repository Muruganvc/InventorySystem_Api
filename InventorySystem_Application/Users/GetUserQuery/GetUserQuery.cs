using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Users.GetUserQuery;
public record GetUserQuery(int UserId) : IRequest<IResult<GetUserQueryResponse>>;
