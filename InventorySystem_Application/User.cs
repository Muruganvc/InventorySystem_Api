namespace InventorySystem_Application;
public class User
{
    public static InventorySystem_Domain.User Get()
    {
        return InventorySystem_Infrastructure.UserService.GetUserDetails();
    }
}
