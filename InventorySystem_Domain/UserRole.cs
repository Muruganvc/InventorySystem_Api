namespace InventorySystem_Domain;
public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;

    public static UserRole AddRole(int UserId, int RoleId)
    {
        return new UserRole
        {
            UserId = UserId,
            RoleId = RoleId
        };
    }
}
