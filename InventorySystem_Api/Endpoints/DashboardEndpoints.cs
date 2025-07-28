using InventorySystem_Application.Common;
using InventorySystem_Application.Dashboard.GetAuditQuery;
using InventorySystem_Application.Dashboard.GetCompanyWiseIncomeQuery;
using InventorySystem_Application.Dashboard.GetIncomeOrOutcomeSummaryReportQuery;
using InventorySystem_Application.Dashboard.GetProductQuantityQuery;
using InventorySystem_Application.Dashboard.GetTotalProductQuery;
using MediatR;

namespace InventorySystem_Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        // GET: Company-wise income
        app.MapGet("/company-wise-income", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompanyWiseIncomeQuery());
            return Results.Ok(result);
        })
        .WithName("GetCompanyWiseIncome")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get company-wise income";
            operation.Description = "Retrieves total income grouped by company.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetCompanyWiseIncomeQueryResponse>>>(StatusCodes.Status200OK);


        // GET: Total products sold
        app.MapGet("/product-sold", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTotalProductQuery());
            return Results.Ok(result);
        })
        .WithName("GetTotalProductsSold")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get total products sold";
            operation.Description = "Fetches the total count of products sold in the system.";
            return operation;
        })
        .Produces<IResult<GetTotalProductQueryResponse>>(StatusCodes.Status200OK);


        // GET: Product quantity summary
        app.MapGet("/product-quantity", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductQuantityQuery());
            return Results.Ok(result);
        })
        .WithName("GetProductQuantitySummary")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get product quantity summary";
            operation.Description = "Returns available stock quantity for all products.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetProductQuantityQueryResponse>>>(StatusCodes.Status200OK);


        // GET: Income/Outcome Summary Report
        app.MapGet("/income-outcome-summary-report", async (
            DateTime? fromDate,
            DateTime? endDate,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetIncomeOrOutcomeSummaryReportQuery(fromDate, endDate));
            return Results.Ok(result);
        })
        .WithName("GetIncomeOutcomeSummaryReport")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get income/outcome summary report";
            operation.Description = "Returns summarized income and outcome data filtered by optional date range.";
            return operation;
        })
        .Produces<IResult<GetIncomeOrOutcomeSummaryReportQueryResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // GET: Audit log
        app.MapGet("/audit-log", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAuditQuery());
            return Results.Ok(result);
        })
        .WithName("GetAuditLog")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get audit log";
            operation.Description = "Fetches system audit records including field-level changes.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetAuditQueryResponse>>>(StatusCodes.Status200OK);

        return app;
    }
}
