using InventorySystem_Api.Common;
using InventorySystem_Api.Endpoints;
using InventorySystem_Application.Common.Mapper;
using InventorySystem_Domain.Common;
using InventorySystem_Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InventorySystemDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.MapCustomHealthCheck();

app.UseGlobalExceptionHandler();

app.MapUserEndpoints()
   .MapCompanyEndpoints()
   .MapCategoryEndpoints()
   .MapCompanyCategoryProductEndpoints()
   .MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();
