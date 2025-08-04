using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using System.Windows.Input;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                        Message = "Server failure",
                        Exeception = error.Message,
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

    public static void MapCrudEndpoints<TCreateRequest, TUpdateRequest, TId, TCreateResponse, TUpdateResponse, TGetResponse>(
    this WebApplication app, string baseRoute,
    Func<TCreateRequest, IRequest<TCreateResponse>> createCommandFactory,
    Func<TId, TUpdateRequest, IRequest<TUpdateResponse>> updateCommandFactory,
    Func<TId, IRequest<TGetResponse>> getByIdQueryFactory,
    Func<IRequest<IEnumerable<TGetResponse>>> getAllQueryFactory,
    string tag = "Entity", string? policy = null 
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
        .Produces(StatusCodes.Status400BadRequest);

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
        .Produces(StatusCodes.Status400BadRequest);

        var getByIdEndpoint = app.MapGet($"{baseRoute}/{{id}}", async (
            TId id,
            IMediator mediator) =>
        {
            var result = await mediator.Send(getByIdQueryFactory(id));
            return Results.Ok(result);
        })
        .WithName($"Get{tag}ById")
        .WithOpenApi()
        .Produces<TGetResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        var getAllEndpoint = app.MapGet($"{baseRoute}", async (
            IMediator mediator) =>
        {
            var result = await mediator.Send(getAllQueryFactory());
            return Results.Ok(result);
        })
        .WithName($"GetAll{tag}s")
        .WithOpenApi()
        .Produces<IEnumerable<TGetResponse>>(StatusCodes.Status200OK);

        if (!string.IsNullOrEmpty(policy))
        {
            createEndpoint.RequireAuthorization(policy);
            updateEndpoint.RequireAuthorization(policy);
            getByIdEndpoint.RequireAuthorization(policy);
            getAllEndpoint.RequireAuthorization(policy);
        }
    }
}
