namespace InventorySystem_Application.Company.GetCompanyQuery;
public record GetCompanyQueryReponse(int Id, 
    string Name, 
    string? Description, 
    bool IsActive, uint RowVersion,
    string UserName
    );
