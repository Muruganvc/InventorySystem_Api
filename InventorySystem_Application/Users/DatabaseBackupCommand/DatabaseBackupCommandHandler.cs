using InventorySystem_Application.Common;
using InventorySystem_Infrastructure.DataBackup;
using MediatR;
using System.Text;

namespace InventorySystem_Application.Users.DatabaseBackupCommand;
internal sealed class DatabaseBackupCommandHandler

    : IRequestHandler<DatabaseBackupCommand, IResult<StringBuilder>>
{
    public async Task<IResult<StringBuilder>> Handle(DatabaseBackupCommand request, CancellationToken cancellationToken)
    {
        var result = await Task.Run(() => PostgresBackup.GenerateBackup(request.connectionString));
        return Result<StringBuilder>.Success(result);
    }
}
