using InventorySystem_Application.Common;
using MediatR;
using System.Text;

namespace InventorySystem_Application.Users.DatabaseBackupCommand
{
    public record DatabaseBackupCommand(string connectionString): IRequest<IResult<(StringBuilder script, bool status)>>;
}
