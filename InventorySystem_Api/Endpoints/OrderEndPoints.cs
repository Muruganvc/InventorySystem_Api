using InventorySystem_Api.Request;
using InventorySystem_Application.Common;
using InventorySystem_Application.Order.GetCustomerOrderSummaryQuery;
using InventorySystem_Application.Order.GetOrdersummaryQuery;
using InventorySystem_Application.Order.OrderCreateCommand;
using InventorySystem_Application.PaymentHistory.CreatePaymentHistoryCommand;
using InventorySystem_Application.PaymentHistory.GetPaymentHistoryQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_Api.Endpoints;

public static class OrderEndPoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        // Create a new order
        app.MapPost("/new-order", async ([FromBody] OrderCreateRequest order, IMediator mediator) =>
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
                item.SerialNo,
                item.Meter
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
        }).RequireAuthorization("AllRoles")
        .WithName("CreateOrder")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create a new order";
            operation.Description = "Creates a customer order with product details, pricing, and optional GST.";
            return operation;
        })
        .Produces<IResult<int>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Get full order summary by ID
        app.MapGet("/order-summary", async ([FromQuery] int orderId, IMediator mediator) =>
        {
            var query = new GetOrdersummaryQuery(orderId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        }).RequireAuthorization("AllRoles")
        .WithName("GetOrderSummary")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get order summary by ID";
            operation.Description = "Fetches a detailed summary of an order using its order ID.";
            return operation;
        })
        .Produces<IResult<GetOrdersummaryQueryResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Get summary of all customer orders
        app.MapGet("/customer-orders", async (IMediator mediator) =>
        {
            var query = new GetCustomerOrderSummaryQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
        }).RequireAuthorization("AllRoles")
        .WithName("GetCustomerOrderSummary")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all customer orders summary";
            operation.Description = "Retrieves summarized information for all customer orders placed.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetCustomerOrderSummaryQueryResponse>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        //Create a payment history record
        app.MapPost("/payment-history", async ([FromBody] CreatePaymentHistoryRequest payment, IMediator mediator) =>
        {
            var command = new CreatePaymentHistoryCommand(
                payment.CustomerId,
                payment.OrderId,
                payment.PaymentMethod,
                payment.AmountPaid,
                payment.BalanceRemainingToPay,
                payment.TransactionRefNo
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("AllRoles").WithName("CreatePaymentHistory")
                .WithOpenApi(operation =>
                {
                    operation.Summary = "Create a payment history record";
                    operation.Description = "Creates a new payment history entry for a customer's order.";
                    return operation;
                })
                .Produces<IResult<bool>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

        //Get payment history by order ID
        app.MapGet("/payment-history/{orderId}", async (int orderId, IMediator mediator) =>
        {
            var query = new GetPaymentHistoryQuery(orderId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        }).RequireAuthorization("AllRoles").WithName("GetPaymentHistory")
                 .WithOpenApi(operation =>
                 {
                     operation.Summary = "Get payment history by order ID";
                     operation.Description = "Retrieves the payment history for a specific customer order by order ID.";
                     return operation;
                 })
                 .Produces<IResult<IReadOnlyList<GetPaymentHistoryQueryResponse>>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
