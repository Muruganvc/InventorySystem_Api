namespace InventorySystem_Domain;
public class Customer
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public uint RowVersion { get; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
