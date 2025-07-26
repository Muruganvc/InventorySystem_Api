using InventorySystem_Api.Request;
using InventorySystem_Application.Product.CreateProductCommand;
using InventorySystem_Application.Product.GetProductQuery;
using InventorySystem_Application.Product.GetProductsQuery;
using InventorySystem_Application.Product.SetActiveInactiveCommand;
using InventorySystem_Application.Product.UpdateProductCommand;
using InventorySystem_Application.Product.UpdateProductQuantityCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_Api.Endpoints;

public static class ProductEndPoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        // Create Product
        app.MapPost("/product", async (
            [FromBody] CreateProductRequest request,
            IMediator mediator) =>
        {
            var command = new CreateProductCommand(request.ProductName,
                request.ProductCategoryId, request.Description, request.Mrp,
                request.SalesPrice, request.Quantity, request.LandingPrice, request.IsActive);

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateProduct")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Update Product
        app.MapPut("/product/{productId}", async (
            int productId,
            [FromBody] UpdateProductRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateProductCommand(productId, request.ProductName,
                request.ProductCategoryId, request.Description, request.Mrp,
                request.SalesPrice, request.Quantity, request.LandingPrice, request.IsActive,request.RowVersion);

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateProduct")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Get Product By Id
        app.MapGet("/Product/{productId}", async (
            int productId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductQuery(productId));
            return Results.Ok(result);
        })
        .WithName("GetProductIdById")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Get All Products
        app.MapGet("/products/{type}", async (string type,IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductsQuery(type));
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Update Product Quantity
        app.MapPut("/product/{productId}/quantity", async (
            int productId,
            [FromBody] UpdateProductQuantityRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateProductQuantityCommand(productId,request.Quantity,request.RowVersion);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateProductQuantity")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Update Product
        app.MapPut("/products/{productId}/status", async (
            int productId,
            [FromBody] UpdateProductStatusRequest request,
            IMediator mediator) =>
        {
            var command = new SetActiveInactiveCommand(productId,request.RowVersion);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateProductStatus")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        return app;
    }
}