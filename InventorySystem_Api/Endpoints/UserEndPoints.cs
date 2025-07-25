using InventorySystem_Application.Users.GetUsersQuery;
using MediatR;

namespace InventorySystem_Api.Endpoints;
public static class UserEndPoints
{
    /// <summary>
    /// Maps the user-related API endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The modified <see cref="IEndpointRouteBuilder"/> with user routes mapped.</returns>
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (IMediator mediator) =>
        {
            var query = new GetUsersQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
        }).WithOpenApi();

        return app;
    }
}
