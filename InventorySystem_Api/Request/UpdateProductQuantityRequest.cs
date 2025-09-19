namespace InventorySystem_Api.Request;
public record UpdateProductQuantityRequest(int Quantity,int Meter, uint RowVersion);
