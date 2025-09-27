using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.DatabaseBackupHistoryCommand.DatabaseBackupHistoryQuery;

public record DatabaseBackupHistoryQuery
    : IRequest<IResult<IReadOnlyList<DatabaseBackupHistoryQueryResponse>>>;