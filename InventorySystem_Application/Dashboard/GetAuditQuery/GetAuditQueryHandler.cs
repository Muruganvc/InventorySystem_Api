using InventorySystem_Domain.Common;
using InventorySystem_Domain;
using InventorySystem_Application.Common;
using MediatR;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Dashboard.GetAuditQuery;

internal sealed class GetAuditQueryHandler
    : IRequestHandler<GetAuditQuery, IResult<IReadOnlyList<GetAuditQueryResponse>>>
{
    private readonly IRepository<AuditLog> _auditRepository;
    public GetAuditQueryHandler(IRepository<AuditLog> auditRepository) => _auditRepository = auditRepository;

    public async Task<IResult<IReadOnlyList<GetAuditQueryResponse>>> Handle(GetAuditQuery request, CancellationToken cancellationToken)
    {
        var auditLogs = await _auditRepository.Table.AsNoTracking().ToListAsync(cancellationToken);

        var response = auditLogs.Select(a => new GetAuditQueryResponse(
            a.AuditLogId,
            a.TableName ?? string.Empty,
            a.Action ?? string.Empty,
            a.ChangedBy ?? string.Empty,
            a.ChangedAt,
            DeserializeJson(a.KeyValues),
            DeserializeJson(a.OldValues),
            DeserializeJson(a.NewValues)
        )).ToList();

        return Result<IReadOnlyList<GetAuditQueryResponse>>.Success(response);
    }

    private static Dictionary<string, object>? DeserializeJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize<Dictionary<string, object>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
