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
        app.MapGet("/company-wise-income", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCompanyWiseIncomeQuery());
            return Results.Ok(result);
        })
        .WithName("GetCompanywiseIncome")
        .WithOpenApi()
        .Produces(200);

        app.MapGet("/product-sold", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTotalProductQuery());
            return Results.Ok(result);
        })
        .WithName("GetProductSoldOut")
        .WithOpenApi()
        .Produces(200);

        app.MapGet("/product-quantity", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductQuantityQuery());
            return Results.Ok(result);
        })
        .WithName("GetProductQuantity")
        .WithOpenApi()
        .Produces(200);

        app.MapGet("/income-outcome-summary-report", async (
            DateTime? fromDate,
            DateTime? endDate,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetIncomeOrOutcomeSummaryReportQuery(fromDate, endDate));
            return Results.Ok(result);
        })
        .WithName("GetIncomeOutcomeSummary")
        .WithOpenApi()
        .Produces(200);

        app.MapGet("/audit-log", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAuditQuery());
            return Results.Ok(result);
        }).WithName("GetAuditlog")
        .WithOpenApi()
        .Produces(200);

        return app;
    }
}
