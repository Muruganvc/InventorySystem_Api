using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.InventoryCompanyInfo.CreateInventoryCompanyInfoCommand;
public record CreateInventoryCompanyInfoCommand(
    string InventoryCompanyInfoName,
    string Description,
    string Address,
    string MobileNo,
    string GstNumber,
    string ApiVersion,
    string UiVersion,
    byte[] QrCode,
    string Email,
    string BankName,
    string BankBranchName,
    string BankAccountNo,
    string BankBranchIFSC)
    :IRequest<IResult<bool>>;
