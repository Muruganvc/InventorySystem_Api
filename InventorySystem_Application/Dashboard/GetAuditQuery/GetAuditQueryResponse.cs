namespace InventorySystem_Application.Dashboard.GetAuditQuery;

public record GetAuditQueryResponse(int Id,
    string TableName,
    string Action,
    string ChangedBy,
    DateTime ChangedAt,
    Dictionary<string, object>? KeyValues,
    Dictionary<string, object>? OldValues,
    Dictionary<string, object>? NewValues);
 