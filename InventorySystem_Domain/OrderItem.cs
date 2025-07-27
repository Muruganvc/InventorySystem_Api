namespace InventorySystem_Domain;
public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string? SerialNo { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal SubTotal { get; private set; } = 0;
    public decimal DiscountAmount { get; private set; }
    public decimal NetTotal { get; private set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public User CreatedUser { get; set; } = null!;
    public uint RowVersion { get; }
}
