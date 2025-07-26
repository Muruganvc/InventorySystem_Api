namespace InventorySystem_Domain;
public class InventoryCompanyInfo
{
    public int InventoryCompanyInfoId { get; set; }
    public string InventoryCompanyInfoName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string MobileNo { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string GstNumber { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public string BankBranchName { get; set; } = null!;
    public string BankAccountNo { get; set; } = null!;
    public string BankBranchIFSC { get; set; } = null!;
    public string ApiVersion { get; set; } = null!;
    public string UiVersion { get; set; } = null!;
    public byte[]? QrCode { get; set; }
    public uint RowVersion { get; }
    public static InventoryCompanyInfo Create(string inventoryCompanyInfoName,
        string description, string address, string mobileNo, string email, string gstNumber, string bankName,
        string bankBranchName, string bankAccountNo, string bankBranchIFSC, string apiVersion, string uiVersion,
        byte[] qrCode)
    {
        return new InventoryCompanyInfo
        {
            Address = address,
            MobileNo = mobileNo,
            Email = email,
            InventoryCompanyInfoName = inventoryCompanyInfoName,
            Description = description,
            GstNumber = gstNumber,
            BankName = bankName,
            BankBranchName = bankBranchName,
            BankAccountNo = bankAccountNo,
            BankBranchIFSC = bankBranchIFSC,
            ApiVersion = apiVersion,
            UiVersion = uiVersion,
            QrCode = qrCode
        };
    }

    public void Update(string inventoryCompanyInfoName,
    string description, string address, string mobileNo, string email, string gstNumber, string bankName,
    string bankBranchName, string bankAccountNo, string bankBranchIFSC, string apiVersion, string uiVersion,
    byte[] qrCode)
    {
        Address = address;
        MobileNo = mobileNo;
        Email = email;
        InventoryCompanyInfoName = inventoryCompanyInfoName;
        Description = description;
        GstNumber = gstNumber;
        BankName = bankName;
        BankBranchName = bankBranchName;
        BankAccountNo = bankAccountNo;
        BankBranchIFSC = bankBranchIFSC;
        ApiVersion = apiVersion;
        UiVersion = uiVersion;
        QrCode = qrCode;
    }
}