using InventorySystem_Api.Common;
using InventorySystem_Api.Endpoints;
using InventorySystem_Application.Common;
using InventorySystem_Application.Common.Mapper;
using InventorySystem_Domain.Common;
using InventorySystem_Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Load JWT settings section
var jwtSection = config.GetSection("JwtSettings");

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<InventorySystemDbContext>(options =>
    options.UseNpgsql(
        config.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 0
            );
        }));

// Register repositories and unit of work
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserInfo, UserInfo>();

// Add AutoMapper with profile(s)
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, typeof(MappingProfile).Assembly);

// Add MediatR and scan assemblies starting with "InventorySystem_"
var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => !a.IsDynamic && a.FullName!.StartsWith("InventorySystem_"))
    .ToArray();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(assemblies);
});

// Configure Authentication with JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = ClaimTypes.Role,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                // IMPORTANT: Add CORS headers here to avoid CORS blocking for 401 response
                var origin = context.Request.Headers["Origin"].ToString();
                if (!string.IsNullOrEmpty(origin))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                }

                context.HandleResponse(); // Prevent default behavior

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = new
                {
                    IsSuccess = false,
                    Error = "Token has expired or is invalid."
                };

                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
            }
        };
    });

// Configure Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllRoles", policy => policy.RequireRole("ADMIN", "SUPERADMIN", "USER"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN", "SUPERADMIN"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("USER"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SUPERADMIN"));
});

// Configure CORS - ensure the exact frontend origin is allowed
builder.Services.AddCustomCors(builder.Environment.IsDevelopment()
    ? "http://localhost:4200"
    : "https://vennilaelectricals.onrender.com");

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

var app = builder.Build();

// Middleware pipeline order is crucial!

app.UseRouting();

// Enable CORS early - before auth middlewares
app.UseCors("AllowFrontend");

// Global Exception Handler (custom middleware)
app.UseGlobalExceptionHandler();

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Swagger middleware (optional in production, typically in dev)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Health checks (if any)
app.MapCustomHealthCheck();

// Map API endpoints modularly
app.MapUserEndpoints()
   .MapCompanyEndpoints()
   .MapCategoryEndpoints()
   .MapCompanyCategoryProductEndpoints()
   .MapProductEndpoints()
   .MapDashboardEndpoints()
   .MapOrderEndpoints();

app.Run();
