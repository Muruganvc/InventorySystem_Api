using InventorySystem_Api.Request;
using InventorySystem_Application.Category.CreateCategoryCommand;
using InventorySystem_Application.Category.GetCategoriesQuery;
using InventorySystem_Application.Category.GetCategoryQuery;
using InventorySystem_Application.Category.UpdateCategoryCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_Api.Endpoints;

public static class CategoryEndPoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        // Create Category
        app.MapPost("/categories", async (
            [FromBody] CreateCategoryRequest request,
            IMediator mediator) =>
        {
            var command = new CreateCategoryCommand(
                request.CategoryName,
                request.CompanyId,
                request.Description,
                request.IsActive
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateCategory")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Update Category
        app.MapPut("/categories/{categoryId}", async (
            int categoryId,
            [FromBody] UpdateCategoryRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateCategoryCommand(
                categoryId,
                request.CategoryName,
                request.CompanyId,
                request.Description,
                request.IsActive,
                request.RowVersion
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateCategory")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Get Category By Id
        app.MapGet("/categories/{categoryId}", async (
            int categoryId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCategoryQuery(categoryId));
            return Results.Ok(result);
        })
        .WithName("GetCategoryById")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // Get All Categories
        app.MapGet("/categories", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCategoriesQuery());
            return Results.Ok(result);
        })
        .WithName("GetCategories")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        return app;
    }
}
