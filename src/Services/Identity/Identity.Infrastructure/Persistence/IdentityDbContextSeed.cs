using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Persistence;

public class IdentityDbContextSeed
{
    public static async Task SeedAsync(IdentityDbContext context, ILogger logger)
    {
        try
        {
            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();

            // Seed Roles if they don't exist
            if (!await context.Roles.AnyAsync())
            {
                var roles = new[]
                {
                    new Role("Admin", "System Administrator with full access", RoleType.Admin),
                    new Role("Manager", "Store Manager with management access", RoleType.Manager),
                    new Role("Cashier", "Cashier with basic POS access", RoleType.Cashier)
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded {Count} roles to database", roles.Length);
            }

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
