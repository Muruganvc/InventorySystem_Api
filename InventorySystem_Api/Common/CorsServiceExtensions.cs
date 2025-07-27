using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;

namespace InventorySystem_Api.Common;
public static class CorsServiceExtensions
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
    public static IServiceCollection AddCustomCors(this IServiceCollection services, string origin)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(origin)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });
        return services;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Inventory System API",
                Description = "API documentation for Inventory Management System",
                // Contact = new OpenApiContact
                // {
                //     Name = "Company Support",
                //     Email = "vcmuruganmca@gmail.com",
                //     Url = new Uri("https://www.facebook.com/vcmuruganmca")
                // }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token in the format: **Bearer &lt;your_token&gt;**"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            options.TagActionsBy(api =>
            {
                return new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default" };
            });

            options.DocInclusionPredicate((_, api) => true);
        });

        return services;
    }

}
