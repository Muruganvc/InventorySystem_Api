namespace InventorySystem_Domain;
public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int ProductCategoryId { get; set; }
    public string? Description { get; set; }
    public decimal MRP { get; set; }
    public decimal SalesPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LandingPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int? ModifiedBy { get; set; }

    // Navigation properties
    public ProductCategory ProductCategory { get; set; } = default!;
    public User CreatedByUser { get; set; } = default!;
    public User? ModifiedByUser { get; set; }

    public static Product Create( string productName, int productCategoryId,
        string? description, decimal mrp, decimal salesPrice, int quantity,
        decimal landingPrice, int createdBy)
    {
        return new Product
        {
            ProductName = productName,
            ProductCategoryId = productCategoryId,
            Description = description,
            MRP = mrp,
            SalesPrice = salesPrice,
            Quantity = quantity,
            LandingPrice = landingPrice,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
    public void Update( string productName, int productCategoryId, string? description,
        decimal mrp, decimal salesPrice, int quantity, decimal landingPrice, bool isActive, int updatedBy)
    {
        ProductName = productName;
        ProductCategoryId = productCategoryId;
        Description = description;
        MRP = mrp;
        SalesPrice = salesPrice;
        Quantity = quantity;
        LandingPrice = landingPrice;
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = updatedBy;
    }

}
