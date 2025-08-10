using InventorySystem_Application.InventoryCompanyInfo.GetInventoryCompanyInfoQuery;

namespace InventorySystem_Application.Users.LoginCommand;
public record LoginCommandResponse(int UserId, string FirstName, string LastName, string Email, string UserName, string Token,
    GetInventoryCompanyInfoQueryResponse? InvCompanyInfo, string RefreshToken);