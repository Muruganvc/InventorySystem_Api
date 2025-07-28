using InventorySystem_Api.Request;
using InventorySystem_Application.Common;
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
        app.MapPost("/product", async ([FromBody] CreateProductRequest request, IMediator mediator) =>
        {
            var command = new CreateProductCommand(
                request.ProductName,
                request.ProductCategoryId,
                request.Description,
                request.Mrp,
                request.SalesPrice,
                request.Quantity,
                request.LandingPrice,
                request.IsActive
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateProduct")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create a new product";
            operation.Description = "Adds a new product with pricing, quantity, and category details.";
            return operation;
        })
        .Produces<IResult<int>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Update Product
        app.MapPut("/product/{productId:int}", async (int productId, [FromBody] UpdateProductRequest request, IMediator mediator) =>
        {
            var command = new UpdateProductCommand(
                productId,
                request.ProductName,
                request.ProductCategoryId,
                request.Description,
                request.Mrp,
                request.SalesPrice,
                request.Quantity,
                request.LandingPrice,
                request.IsActive,
                request.RowVersion
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateProduct")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update product details";
            operation.Description = "Updates existing product information including quantity and pricing.";
            return operation;
        })
        .Produces<IResult<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Get Product by ID
        app.MapGet("/product/{productId:int}", async (int productId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductQuery(productId));
            return Results.Ok(result);
        })
        .WithName("GetProductById")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get product by ID";
            operation.Description = "Retrieves the product details for the specified product ID.";
            return operation;
        })
        .Produces<IResult<GetProductQueryResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Get All Products by Type
        app.MapGet("/products/{type}", async (string type, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductsQuery(type));
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all products by type";
            operation.Description = "Returns all products filtered by type such as active or inactive.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetProductsQueryResponse>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Update Product Quantity
        app.MapPut("/product/{productId:int}/quantity", async (int productId, [FromBody] UpdateProductQuantityRequest request, IMediator mediator) =>
        {
            var command = new UpdateProductQuantityCommand(productId, request.Quantity, request.RowVersion);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateProductQuantity")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update product quantity";
            operation.Description = "Updates the available stock quantity for a product.";
            return operation;
        })
        .Produces<IResult<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Set Product Active/Inactive
        app.MapPut("/products/{productId:int}/status", async (int productId, [FromBody] UpdateProductStatusRequest request, IMediator mediator) =>
        {
            var command = new SetActiveInactiveCommand(productId, request.RowVersion);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateProductStatus")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Change product status";
            operation.Description = "Marks the specified product as active or inactive.";
            return operation;
        })
        .Produces<IResult<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
