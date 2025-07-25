using InventorySystem_Infrastructure.TableConfiguration;
using Microsoft.EntityFrameworkCore;
namespace InventorySystem_Infrastructure;
public class InventorySystemDbContext : DbContext
{
    public InventorySystemDbContext(DbContextOptions<InventorySystemDbContext> options)
       : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}
