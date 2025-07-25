namespace InventorySystem_Domain;
public class Company //: BaseEntity
{
    // EF Core requires a parameterless constructor
    private Company() { }
    private Company(string companyName, int createdBy, string? description = null)
    {
        CompanyName = companyName;
        Description = description; 
        CreatedBy = createdBy;
    }
    public int CompanyId { get; private set; }
    public string CompanyName { get;  set; } = default!;
    public string? Description { get;  set; }
    public bool IsActive { get;  set; } = true;  
    public User CreatedByUser { get; private set; } = default!;
    public User? ModifiedByUser { get; private set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int? ModifiedBy { get; set; }
    public uint RowVersion { get; private set; }
    public ICollection<Category> Categories { get; private set; } = new List<Category>();
    public static Company Create(string companyName, int createdBy, string? description = null)
    {
        return new Company(companyName, createdBy, description);
    } 
    public void Update(string name, string? description, int modifiedBy)
    {
        CompanyName = name;
        Description = description;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
        //UpdateMetadata(modifiedBy);
    }
}
