using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Infraestructure.DataAcess;

namespace MyRecipeBook.Infraestructure.Migrations;

public class DatabaseMigration
{
    public static async Task ExecuteMigrations(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<MyRecipeBookDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
