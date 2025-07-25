namespace InventorySystem_Domain;
public class Category
{
    public int CategoryId { get; private set; }
    public string CategoryName { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public int? ModifiedBy { get; private set; }

    public int? CompanyId { get; private set; }
    public Company Company { get; private set; } = default!;
    public User CreatedByUser { get; private set; } = default!;
    public User? ModifiedByUser { get; private set; }
    public uint RowVersion { get; }
    public ICollection<ProductCategory> ProductCategories { get; private set; } = new List<ProductCategory>();

    // Constructor for EF
    private Category() { }

    public static Category Create(string name, int companyId, int createdBy, string? description = null, bool isActive = false)
    {
        return new Category
        {
            CategoryName = name,
            Description = description,
            CompanyId = companyId,
            IsActive = isActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
    }
    public void Update(string name, int companyId, string? description, bool isActive, int modifiedBy)
    {
        CategoryName = name;
        Description = description;
        CompanyId = companyId;
        IsActive = isActive;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}
