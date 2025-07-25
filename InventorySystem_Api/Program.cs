﻿using InventorySystem_Api.Common;
using InventorySystem_Api.Endpoints;
using InventorySystem_Application.Common.Mapper;
using InventorySystem_Domain.Common;
using InventorySystem_Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var jwtSection = config.GetSection("JwtSettings");
builder.Services.AddDbContext<InventorySystemDbContext>(options =>
    options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllRoles", policy => policy.RequireRole("ADMIN", "SUPERADMIN", "User"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN", "SUPERADMIN"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("USER"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SUPERADMIN"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{ 
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Inventory System API",
        Description = "API documentation for Inventory Management System",
        //Contact = new OpenApiContact
        //{
        //    Name = "Company Support",
        //    Email = "vcmuruganmca@gmail.com",
        //    Url = new Uri("https://www.facebook.com/vcmuruganmca")
        //} 
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
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


var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowCors");
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
