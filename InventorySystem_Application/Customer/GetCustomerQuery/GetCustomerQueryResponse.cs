namespace InventorySystem_Application.Customer.GetCustomerQuery;
public record GetCustomerQueryResponse(int CustomerId,
    string CustomerName,
    string MobileNo,
    string Address)
{
    internal object ToListAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
