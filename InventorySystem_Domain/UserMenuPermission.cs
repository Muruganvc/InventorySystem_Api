namespace InventorySystem_Domain;

public class UserMenuPermission
{
    public int UserMenuPermissionId { get; set; }
    public int UserId { get; set; }
    public int MenuItemId { get; set; }
    public int? OrderBy { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
    public uint RowVersion { get; }

    public static UserMenuPermission AddUserMenu(int userId, int menuId, int orderBy)
    {
        return new UserMenuPermission
        {
            UserId = userId,
            MenuItemId= menuId,
            OrderBy = orderBy
        };
    }

}
