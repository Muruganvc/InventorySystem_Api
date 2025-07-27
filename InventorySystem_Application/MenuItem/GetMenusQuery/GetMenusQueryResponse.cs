namespace InventorySystem_Application.MenuItem.GetMenusQuery;
public record GetMenusQueryResponse(
    int Id,
    int ParentId,
    int OrderBy,
    string Label,
    string Icon,
    string Route,
    List<GetMenusQueryResponse>? SubMenuItem
);
