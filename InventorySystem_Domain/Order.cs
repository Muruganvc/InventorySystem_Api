namespace InventorySystem_Domain;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal? TotalAmount { get; set; }
    public decimal? FinalAmount { get; set; }
    public decimal? BalanceAmount { get; set; }
    public bool IsGst { get; set; } = false;
    public string? GstNumber { get; set; }
    public uint RowVersion { get; }
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}