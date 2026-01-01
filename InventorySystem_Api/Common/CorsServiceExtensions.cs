using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;

namespace InventorySystem_Api.Common;

public static class CorsServiceExtensions
{
    /// <summary>
    /// Global exception handler middleware
    /// </summary>
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
                        Message = "Server failure",
                        Exception = error.Message,
#if DEBUG
                        StackTrace = error.StackTrace
#else
                        Detailed = error.InnerException?.Message
#endif
                    });

                    await context.Response.WriteAsync(result);
                }
            });
        });
    }

    /// <summary>
    /// Add CORS policy for frontend origins
    /// </summary>
    public static IServiceCollection AddCustomCors(this IServiceCollection services, params string[] origins)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(origins)      // ✅ multiple origins supported
                      .AllowAnyHeader()         // ✅ allow all headers (including Authorization)
                      .AllowAnyMethod()         // ✅ allow all HTTP methods
                      .AllowCredentials();      // ✅ allow cookies / auth headers
            });
        });

        return services;
    }


    /// <summary>
    /// Add Swagger/OpenAPI with JWT Bearer support
    /// </summary>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Inventory System API",
                Description = "API documentation for Inventory Management System"
            });

            // XML comments
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // JWT Auth
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token as: Bearer <your_token>"
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

            // Tag actions by controller or group name
            options.TagActionsBy(api =>
                new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default" });

            options.DocInclusionPredicate((_, api) => true);
        });

        return services;
    }

    /// <summary>
    /// Generic CRUD endpoint mapper using MediatR
    /// </summary>
    public static void MapCrudEndpoints<TCreateRequest, TUpdateRequest, TId, TCreateResponse, TUpdateResponse, TGetResponse>(
        this WebApplication app,
        string baseRoute,
        Func<TCreateRequest, IRequest<TCreateResponse>> createCommandFactory,
        Func<TId, TUpdateRequest, IRequest<TUpdateResponse>> updateCommandFactory,
        Func<TId, IRequest<TGetResponse>> getByIdQueryFactory,
        Func<IRequest<IEnumerable<TGetResponse>>> getAllQueryFactory,
        string tag = "Entity",
        string? policy = null
    )
    {
        var createEndpoint = app.MapPost($"{baseRoute}", async (
            [FromBody] TCreateRequest request,
            IMediator mediator) =>
        {
            var result = await mediator.Send(createCommandFactory(request));
            return Results.Ok(result);
        })
        .WithName($"Create{tag}")
        .WithOpenApi()
        .Produces<TCreateResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        var updateEndpoint = app.MapPut($"{baseRoute}/{{id}}", async (
            TId id,
            [FromBody] TUpdateRequest request,
            IMediator mediator) =>
        {
            var result = await mediator.Send(updateCommandFactory(id, request));
            return Results.Ok(result);
        })
        .WithName($"Update{tag}")
        .WithOpenApi()
        .Produces<TUpdateResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        var getByIdEndpoint = app.MapGet($"{baseRoute}/{{id}}", async (
            TId id,
            IMediator mediator) =>
        {
            var result = await mediator.Send(getByIdQueryFactory(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName($"Get{tag}ById")
        .WithOpenApi()
        .Produces<TGetResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        var getAllEndpoint = app.MapGet($"{baseRoute}", async (
            IMediator mediator) =>
        {
            var result = await mediator.Send(getAllQueryFactory());
            return Results.Ok(result);
        })
        .WithName($"GetAll{tag}s")
        .WithOpenApi()
        .Produces<IEnumerable<TGetResponse>>(StatusCodes.Status200OK);

        // Apply authorization policy if provided
        if (!string.IsNullOrEmpty(policy))
        {
            createEndpoint.RequireAuthorization(policy);
            updateEndpoint.RequireAuthorization(policy);
            getByIdEndpoint.RequireAuthorization(policy);
            getAllEndpoint.RequireAuthorization(policy);
        }
    }
}
