using InventorySystem_Infrastructure;

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
                    return Results.Json(new
                    {
                        status = "Healthy",
                        message = "Database is up and running!"
                    }, statusCode: 200);
                }
                else
                {
                    return Results.Json(new
                    {
                        status = "Unhealthy",
                        message = "Unable to connect to the database."
                    }, statusCode: 503);
                }
            }
            catch (Exception ex)
            {
                return Results.Json(new
                {
                    status = "Error",
                    message = "Exception occurred while checking database health.",
                    error = ex.Message
                }, statusCode: 500);
            }
        });
        return app;
    }
}
