using InventorySystem_Api.Common;
using InventorySystem_Api.Endpoints;
using InventorySystem_Application.Common;
using InventorySystem_Application.Common.Mapper;
using InventorySystem_Domain.Common;
using InventorySystem_Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var jwtSection = config.GetSection("JwtSettings");
builder.Services.AddDbContext<InventorySystemDbContext>(options =>
    options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserInfo, UserInfo>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, typeof(MappingProfile).Assembly);

var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => !a.IsDynamic && a.FullName!.StartsWith("InventorySystem_"))
    .ToArray();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(assemblies);
});

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

        // Custom error handling for unauthorized access
        options.Events = new JwtBearerEvents
        {
            // This event is triggered when authentication fails (e.g., expired token)
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },

            // This event is triggered when the user is not authorized (i.e., missing/invalid token)
            OnChallenge = async context =>
            {
                // Set the response status code to 401 (Unauthorized)
                context.Response.StatusCode = 401;

                // Set the content type to application/json
                context.Response.ContentType = "application/json";

                // Write the custom response with Result<T> structure
                var result = new
                {
                    IsSuccess = false,
                    Error = "Token has expired or is invalid."
                };

                // Writing the response asynchronously
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
            }
        };
    });





builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllRoles", policy => policy.RequireRole("ADMIN", "SUPERADMIN", "User"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN", "SUPERADMIN"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("USER"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SUPERADMIN"));
});

//if (builder.Environment.IsDevelopment())
//{
//    builder.Services.AddCustomCors("http://localhost:4200");
//}
//else
//{
//    builder.Services.AddCustomCors("https://vennilaelectricals-qa.onrender.com");
//}
builder.Services.AddCustomCors("https://vennilaelectricals-qa.onrender.com");

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCustomSwagger();

var app = builder.Build();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Health Check (if implemented)
app.MapCustomHealthCheck();

// Modular API Endpoints
app.MapUserEndpoints()
   .MapCompanyEndpoints()
   .MapCategoryEndpoints()
   .MapCompanyCategoryProductEndpoints()
   .MapProductEndpoints()
   .MapDashboardEndpoints()
   .MapOrderEndpoints();

app.Run();
