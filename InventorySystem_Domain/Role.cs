namespace InventorySystem_Domain;
public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = default!;
    public string RoleCode { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

}
