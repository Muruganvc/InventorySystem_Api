using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace InventorySystem_Api.Common;

public static class ExceptionHandlerExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature?.Error is Exception error)
                {
                    var result = JsonSerializer.Serialize(new
                    {
                        context.Response.StatusCode,
                        error.Message,
                        Detailed = error.InnerException?.Message
                    });

                    await context.Response.WriteAsync(result);
                }
            });
        });
    }
}