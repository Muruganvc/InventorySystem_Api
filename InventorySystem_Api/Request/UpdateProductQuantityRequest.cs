namespace InventorySystem_Api.Request;
public record UpdateProductQuantityRequest(int Quantity, uint RowVersion);
