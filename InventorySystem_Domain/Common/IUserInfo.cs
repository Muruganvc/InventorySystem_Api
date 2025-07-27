namespace InventorySystem_Domain.Common;

public interface IUserInfo
{
    int UserId { get; }
    string Email { get; }
    string UserName { get; }
}
