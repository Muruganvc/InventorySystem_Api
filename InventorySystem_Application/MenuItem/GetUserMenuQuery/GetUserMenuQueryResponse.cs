namespace InventorySystem_Application.MenuItem.GetUserMenuQuery;
public class GetMenuItemQueryResponse
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public int OrderBy { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Route { get; set; }
    public List<GetMenuItemQueryResponse>? SubMenuItem { get; set; }
}