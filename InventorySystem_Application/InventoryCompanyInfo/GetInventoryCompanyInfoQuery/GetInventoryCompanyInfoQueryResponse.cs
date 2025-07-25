namespace InventorySystem_Application.InventoryCompanyInfo.GetInventoryCompanyInfoQuery;
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
    string BankBranchIFSC);
