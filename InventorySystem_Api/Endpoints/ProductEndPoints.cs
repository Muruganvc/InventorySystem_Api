using InventorySystem_Api.Request;
using InventorySystem_Application.Product.CreateProductCommand;
using InventorySystem_Application.Product.GetProductQuery;
using InventorySystem_Application.Product.GetProductsQuery;
using InventorySystem_Application.Product.UpdateProductCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_Api.Endpoints;

public static class ProductEndPoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        // Create Product
        app.MapPost("/products", async (
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
        app.MapPut("/products/{productId}", async (
            int productId,
            [FromBody] UpdateProductRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateProductCommand(productId, request.ProductName,
                request.ProductCategoryId, request.Description, request.Mrp,
                request.SalesPrice, request.Quantity, request.LandingPrice, request.IsActive);

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
        app.MapGet("/products", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductsQuery());
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);
        return app;
    }
}