using InventorySystem_Domain;

namespace InventorySystem_Infrastructure;
public class UserService
{
    public static User GetUserDetails() => new(1, "Muruganvc");
}
