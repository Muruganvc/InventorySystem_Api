using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Order.OrderCreateCommand;
public record CustomerCommand(
    int CustomerId,
    string CustomerName,
    string Phone,
    string? Address
);

public record OrderItemCommand(
    int ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    string? SerialNo
);

public record OrderCreateCommand(CustomerCommand Customer, List<OrderItemCommand> OrderItemRequests, decimal GivenAmount,
    bool IsGst, string GstNumber) : IRequest<IResult<int>>;