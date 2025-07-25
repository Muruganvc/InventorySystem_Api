using InventorySystem_Api.Request;
using InventorySystem_Application.CompanyCategoryProduct.CreateCompanyCategoryProduct;
using InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductQuery;
using InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductsQuery;
using InventorySystem_Application.CompanyCategoryProduct.UpdateCompanyCategoryProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_Api.Endpoints;

public static class CompanyProductCategoryItemEndPoints
{
    public static IEndpointRouteBuilder MapCompanyCategoryProductEndpoints(this IEndpointRouteBuilder app)
    {
        // Create Company Category Product
        app.MapPost("/company-category-product", async (
            [FromBody] CreateCompanyCategoryProductRequest request,
            IMediator mediator) =>
        {
            var command = new CreateCompanyCategoryProductCommand(
                request.CompanyCategoryProductItemName,
                request.CategoryId,
                request.Description,
                request.IsActive
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateCompanyCategoryProduct")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Update Company Category Product
        app.MapPut("/company-category-product/{productItemId}", async (
            int productItemId,
            [FromBody] UpdateCompanyCategoryProductRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateCompanyCategoryProductCommand(
                productItemId,
                request.CompanyCategoryProductItemName,
                request.CategoryId,
                request.Description,
                request.IsActive,
                request.RowVersion
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateCompanyCategoryProduct")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Get Company Category Product by ID
        app.MapGet("/company-category-product/{productCategoryId}", async (
            int productCategoryId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompanyCategoryProductQuery(productCategoryId));
            return Results.Ok(result);
        })
        .WithName("GetCompanyCategoryProductById")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Get All Company Category Products
        app.MapGet("/company-category-products", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompanyCategoryProductsQuery());
            return Results.Ok(result);
        })
        .WithName("GetCompanyCategoryProducts")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        return app;
    }
}
