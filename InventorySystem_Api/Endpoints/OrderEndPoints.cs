using InventorySystem_Api.Request;
using InventorySystem_Application.Order.GetCustomerOrderSummaryQuery;
using InventorySystem_Application.Order.GetOrdersummaryQuery;
using InventorySystem_Application.Order.OrderCreateCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_Api.Endpoints;

public static class OrderEndPoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        // Endpoint: Create a new order
        app.MapPost("/new-order", async (
            [FromBody] OrderCreateRequest order,
            IMediator mediator) =>
        {
            var customerCommand = new CustomerCommand(
                order.Customer.CustomerId,
                order.Customer.CustomerName,
                order.Customer.Phone,
                order.Customer.Address
            );

            var orderItemCommands = order.OrderItemRequests.Select(item => new OrderItemCommand(
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
                item.DiscountPercent,
                item.SerialNo
            )).ToList();

            var command = new OrderCreateCommand(
                customerCommand,
                orderItemCommands,
                order.GivenAmount,
                order.IsGst,
                order.GstNumber
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateOrder")
        .WithOpenApi()
        .Produces(200);

        // Endpoint: Get full order summary for an order ID
        app.MapGet("/order-summary", async (
            [FromQuery] int orderId,
            IMediator mediator) =>
        {
            var query = new GetOrdersummaryQuery(orderId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetOrdersummaryQuery")
        .WithOpenApi()
        .Produces(200);

        // Endpoint: Get summary of all customer orders
        app.MapGet("/customer-orders", async (
            IMediator mediator) =>
        {
            var query = new GetCustomerOrderSummaryQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetCustomerOrderSummaryQuery")
        .WithOpenApi()
        .Produces(200);

        return app;
    }
}
