using InventorySystem_Api.Request;
using InventorySystem_Application.Common;
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
        // POST: Create Company Category Product
        app.MapPost("/company-category-product", async (
            [FromBody] CreateCompanyCategoryProductRequest request,
            IMediator mediator) =>
        {
            var command = new CreateCompanyCategoryProductCommand(
                request.categoryProductName,
                request.CategoryId,
                request.Description,
                request.IsActive
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("AdminOnly")
        .WithName("CreateCompanyCategoryProduct")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create company-category product";
            operation.Description = "Creates a new product item associated with a company and a product category.";
            return operation;
        })
         .Produces<IResult<int>>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status400BadRequest);


        // PUT: Update Company Category Product
        app.MapPut("/company-category-product/{productItemId:int}", async (
            int productItemId,
            [FromBody] UpdateCompanyCategoryProductRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateCompanyCategoryProductCommand(
                productItemId,
                request.ProductCategoryName,
                request.CategoryId,
                request.Description,
                request.IsActive,
                request.RowVersion
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("AdminOnly")
         .WithName("UpdateCompanyCategoryProduct")
         .WithOpenApi(operation =>
         {
             operation.Summary = "Update company-category product";
             operation.Description = "Updates an existing company-category product item by its ID.";
             return operation;
         })
         .Produces<IResult<bool>>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status400BadRequest);



        // GET: Get Company Category Product by ID
        app.MapGet("/company-category-product/{productCategoryId:int}", async (
            int productCategoryId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompanyCategoryProductQuery(productCategoryId));
            return Results.Ok(result);
        }).RequireAuthorization("AdminOnly")
        .WithName("GetCompanyCategoryProductById")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get company-category product by ID";
            operation.Description = "Returns a single company-category product by its ID.";
            return operation;
        })
        .Produces<GetCompanyCategoryProductQueryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // GET: Get all Company Category Products (filtered by active status)
        app.MapGet("/company-category-products/{isAllActive:bool}", async (
            bool isAllActive,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompanyCategoryProductsQuery(isAllActive));
            return Results.Ok(result);
        }).RequireAuthorization("AdminOnly")
        .WithName("GetCompanyCategoryProducts")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all company-category products";
            operation.Description = "Retrieves all company-category product items, optionally filtered by active status.";
            return operation;
        })
        .Produces<IReadOnlyList<GetCompanyCategoryProductsQueryResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        return app;
    }
}
