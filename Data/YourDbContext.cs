using Microsoft.EntityFrameworkCore;
using SyncfusionWebAppTest1.Models;

namespace SyncfusionWebAppTest1.Data
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrdersDetails> OrdersDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrdersDetails>(entity =>
            {
                entity.HasKey(e => e.OrderID);
                entity.Property(e => e.OrderID).ValueGeneratedOnAdd();
                entity.Property(e => e.CustomerID).IsRequired();
                entity.Property(e => e.ShipCity).IsRequired();
                entity.Property(e => e.OrderDate).IsRequired();
                entity.Property(e => e.EmployeeID).IsRequired();
                entity.Property(e => e.Verified).IsRequired();
            });
        }
    }
}