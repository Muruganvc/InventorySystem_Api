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
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // PUT: Update Company
        app.MapPut("/companies/{companyId}", async (
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
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // GET: Get Company by ID
        app.MapGet("/companies/{companyId}", async (
            int companyId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompanyQuery(companyId));
            return Results.Ok(result);
        })
        .WithName("GetCompanyById")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // GET: Get All Companies
        app.MapGet("/companies", async (bool isAllActiveCompany,IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompaniesQuery(isAllActiveCompany));
            return Results.Ok(result);
        })
        .WithName("GetCompanies")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        return app;
    }
}
