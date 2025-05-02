using Microsoft.EntityFrameworkCore;
using SyncfusionWebAppTest1.Data;
using System.IO;

namespace SyncfusionWebAppTest1
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new SyncTestDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<SyncTestDbContext>>());

            // Check if there's already data in the database
            if (context.OrdersDetails.Any())
            {
                return; // Database has already been seeded
            }

            // Read the SQL script from file
            string sql = File.ReadAllText("seed-data.sql");

            // Execute the SQL script directly
            context.Database.ExecuteSqlRaw(sql);
        }
    }
}