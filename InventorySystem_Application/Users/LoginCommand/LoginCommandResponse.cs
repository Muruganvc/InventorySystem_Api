namespace InventorySystem_Application.Users.LoginCommand;
public record GetInventoryCompanyInfoQueryResponse(
    int InventoryCompanyInfoId,
    string InventoryCompanyInfoName,
    string Description,
    string Address,
    string MobileNo,
    string GstNumber,
    string ApiVersion,
    string UiVersion,
    string QrCodeBase64,
    string Email,
    string BankName,
    string BankBranchName,
    string BankAccountNo,
    string BankBranchIFSC
);
public record LoginCommandResponse(int UserId, string FirstName, string LastName, string Email, string UserName, string Token,
    GetInventoryCompanyInfoQueryResponse? InvCompanyInfo);