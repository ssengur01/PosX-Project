using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Products.Domain.Entities;

namespace Products.Infrastructure.Persistence;

public class ProductsDbContextSeed
{
    public static async Task SeedAsync(ProductsDbContext context, ILogger logger)
    {
        try
        {
            await context.Database.MigrateAsync();

            if (!await context.Categories.AnyAsync())
            {
                var categories = new[]
                {
                    new Category("Gıda", "Gıda ürünleri"),
                    new Category("İçecek", "İçecekler ve meşrubatlar"),
                    new Category("Temizlik", "Temizlik malzemeleri"),
                    new Category("Kırtasiye", "Kırtasiye ürünleri"),
                    new Category("Elektronik", "Elektronik ürünler")
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded {Count} categories to database", categories.Length);
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
