namespace InventorySystem_Domain;

public class ProductCategory
{
    public int ProductCategoryId { get; private set; }
    public string ProductCategoryName { get; private set; } = default!;
    public int CategoryId { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public int? ModifiedBy { get; private set; }
    public Category Category { get; private set; } = default!;
    public User CreatedByUser { get; private set; } = default!;
    public User? ModifiedByUser { get; private set; }
    public uint RowVersion { get; }
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public static ProductCategory Create(string name, int categoryId, int createdBy, string? description = null, bool isActive = false)
    {
        return new ProductCategory
        {
            ProductCategoryName = name,
            Description = description,
            CategoryId = categoryId,
            IsActive = isActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
    public void Update(string name, int categoryId, string? description, bool isActive, int modifiedBy)
    {
        ProductCategoryName = name;
        Description = description;
        CategoryId = categoryId;
        IsActive = isActive;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }

}

