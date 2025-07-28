using InventorySystem_Api.Request;
using InventorySystem_Application.Company.CreateCompanyCommand;
using InventorySystem_Application.Company.GetCompaniesQuery;
using InventorySystem_Application.Company.GetCompanyQuery;
using InventorySystem_Application.Company.UpdateCompnayCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        })
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
        })
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
        })
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
        })
        .WithName("GetCompanies")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all companies";
            operation.Description = "Returns a list of companies, filtered by active status if specified.";
            return operation;
        })
        .Produces<IResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        return app;
    }
}
