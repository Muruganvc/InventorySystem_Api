namespace InventorySystem_Application.Users.GetRoleByUserQuery;

public record GetRoleByUserQueryResponse(int RoleId, 
    int UserId, 
    int UserRoleId, 
    string RoleName);
