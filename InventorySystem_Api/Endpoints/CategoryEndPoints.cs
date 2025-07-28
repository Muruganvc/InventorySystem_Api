using InventorySystem_Api.Request;
using InventorySystem_Application.Category.CreateCategoryCommand;
using InventorySystem_Application.Category.GetCategoriesByCompanyQuery;
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
        // POST: Create Category
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
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create a new category";
            operation.Description = "Creates a new category under a specified company.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // PUT: Update Category
        app.MapPut("/categories/{categoryId:int}", async (
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
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update a category";
            operation.Description = "Updates the details of an existing category.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // GET: Get Category By ID
        app.MapGet("/categories/{categoryId:int}", async (
            int categoryId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCategoryQuery(categoryId));
            return Results.Ok(result);
        })
        .WithName("GetCategoryById")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get a category by ID";
            operation.Description = "Fetches details of a category using its ID.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // GET: Get All Categories
        app.MapGet("/categories", async (
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCategoriesQuery());
            return Results.Ok(result);
        })
        .WithName("GetCategories")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all categories";
            operation.Description = "Returns a list of all categories available.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // GET: Get Categories By Company ID
        app.MapGet("/categories/company/{companyId:int}", async (
            int companyId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCategoriesByCompanyQuery(companyId));
            return Results.Ok(result);
        })
        .WithName("GetCategoryByCompany")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get categories by company ID";
            operation.Description = "Returns all categories that belong to a specific company.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
         

        return app;
    }
}
