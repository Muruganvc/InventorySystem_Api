using InventorySystem_Domain;
using InventorySystem_Infrastructure.TableConfiguration;
using Microsoft.EntityFrameworkCore;
namespace InventorySystem_Infrastructure;
public class InventorySystemDbContext : DbContext
{
    public InventorySystemDbContext(DbContextOptions<InventorySystemDbContext> options)
       : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
