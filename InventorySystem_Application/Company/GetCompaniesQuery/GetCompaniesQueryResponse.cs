namespace InventorySystem_Application.Company.GetCompaniesQuery;
public record GetCompaniesQueryResponse(int Id, 
    string Name, 
    string? Description, 
    bool IsActive, uint RowVersion,
    string UserName);
