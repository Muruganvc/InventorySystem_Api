using InventorySystem_Api.Request;
using InventorySystem_Application.Company.BulkCompanyCommand;
using InventorySystem_Application.Company.CreateCompanyCommand;
using InventorySystem_Application.Company.GetCompaniesQuery;
using InventorySystem_Application.Company.GetCompanyQuery;
using InventorySystem_Application.Company.UpdateCompnayCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace InventorySystem_Api.Endpoints;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        // POST: Create Company
        app.MapPost("/companies", async (
            [FromBody] CreateCompanyRequest request,
            IMediator mediator) =>
        {
            var command = new CreateCompanyCommand(
                request.CompanyName,
                request.Description,
                request.IsActive
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("AdminOnly")
        .WithName("CreateCompany")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create a new company";
            operation.Description = "Creates a new company with the specified name, description, and active status.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // PUT: Update Company
        app.MapPut("/companies/{companyId:int}", async (
            int companyId,
            [FromBody] UpdateCompanyRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateCompnayCommand(
                companyId,
                request.CompanyName,
                request.Description,
                request.IsActive,
                request.RowVersion
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireAuthorization("AdminOnly")
        .WithName("UpdateCompany")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update company details";
            operation.Description = "Updates the company with the given ID using the provided data.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // GET: Get Company by ID
        app.MapGet("/companies/{companyId:int}", async (
            int companyId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompanyQuery(companyId));
            return Results.Ok(result);
        }).RequireAuthorization("AllRoles")
        .WithName("GetCompanyById")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get a company by ID";
            operation.Description = "Returns company details for the specified ID.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // GET: Get All Companies (optionally filter by active status)
        app.MapGet("/companies", async (
            [FromQuery] bool isAllActiveCompany,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompaniesQuery(isAllActiveCompany));
            return Results.Ok(result);
        }).RequireAuthorization("AllRoles")
        .WithName("GetCompanies")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all companies";
            operation.Description = "Returns a list of companies, filtered by active status if specified.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);



        app.MapPost("/bulk-company", async (
     List<BulkComapanyRequest> request,
     IMediator mediator) =>
        {
            var bulkCompanyEntries = new List<BulkCompanyEntry>();
            request.ForEach(a =>
            {
                bulkCompanyEntries.Add(new BulkCompanyEntry(
                    a.CompanyName,
                    a.CategoryName,
                    a.ProductCategoryName
                ));
            });
            var command = new BulkCompanyCommand(bulkCompanyEntries);
            var result = await mediator.Send(command);

            return Results.Ok(result);
        })
 .RequireAuthorization("AllRoles")
 .WithName("BulkCompanyCommand")
 .WithOpenApi(operation =>
 {
     operation.Summary = "Bulk Company Upload";
     operation.Description = "Uploads a list of companies in bulk, including their company name, category name, and product category name.";
     operation.Description += " The operation returns a success status along with the processed entries or any validation errors if present.";
     return operation;
 })
 .Produces<IResult>(StatusCodes.Status200OK)
 .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
