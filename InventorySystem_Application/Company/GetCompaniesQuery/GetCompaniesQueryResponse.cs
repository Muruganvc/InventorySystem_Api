namespace InventorySystem_Application.Company.GetCompaniesQuery;
public record GetCompaniesQueryResponse(int CompanyId, 
    string CompanyName, 
    string? Description, 
    bool IsActive, uint RowVersion,
    DateTime CreatedDate,
    string CreatedBy);
