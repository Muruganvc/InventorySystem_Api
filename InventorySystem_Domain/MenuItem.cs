namespace InventorySystem_Domain;

public class MenuItem
{
    public int MenuItemId { get; set; }
    public string Label { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int? ParentId { get; set; }
    public int? OrderBy { get; set; }
    public uint RowVersion { get; }

    // Navigation properties
    public MenuItem? Parent { get; set; }
    public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
    public ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();

}
