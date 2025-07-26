using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Dashboard.GetAuditQuery;

public record GetAuditQuery()
    : IRequest<IResult<IReadOnlyList<GetAuditQueryResponse>>>;
