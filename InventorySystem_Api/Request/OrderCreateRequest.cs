namespace InventorySystem_Api.Request;
public class CustomerRequest
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
}
public class OrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public string? SerialNo { get; set; }
    public int? Meter { get; set; }
}

public record OrderCreateRequest(CustomerRequest Customer, List<OrderItemRequest> OrderItemRequests, decimal GivenAmount, bool IsGst, string GstNumber);
