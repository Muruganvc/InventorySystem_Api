using InventorySystem_Infrastructure;
using InventorySystem_Application.Common;

namespace InventorySystem_Api.Common;

public static class HealthCheckEndpointExtensions
{
    public static IEndpointRouteBuilder MapCustomHealthCheck(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", async (InventorySystemDbContext dbContext) =>
        {
            try
            {
                var canConnect = await dbContext.Database.CanConnectAsync();

                if (canConnect)
                {
                    var response = new HealthCheckResponse("Healthy", "Database is up and running!");
                    return Results.Ok(Result<HealthCheckResponse>.Success(response));
                }

                return Results.Ok(Result<HealthCheckResponse>.Failure("Unable to connect to the database."));
            }
            catch (Exception ex)
            {
                return Results.Ok(Result<HealthCheckResponse>.Failure($"An error occurred while checking database health: {ex.Message}"));
            }
        })
        .WithName("HealthCheck")
        .WithTags("System")
        .Produces<Result<HealthCheckResponse>>(200)
        .Produces<Result<HealthCheckResponse>>(503)
        .Produces<Result<HealthCheckResponse>>(500);

        return app;
    }
}

public record HealthCheckResponse(string Status, string Message);
