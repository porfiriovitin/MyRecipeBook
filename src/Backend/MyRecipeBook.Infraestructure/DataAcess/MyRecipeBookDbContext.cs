using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using System.Runtime.CompilerServices;

[assembly:  InternalsVisibleTo("WebApi.Tests")]
namespace MyRecipeBook.Infraestructure.DataAcess;

internal class MyRecipeBookDbContext : DbContext
{
    public MyRecipeBookDbContext(DbContextOptions<MyRecipeBookDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Recipe>().HasOne<User>().WithMany().HasForeignKey(recipe => recipe.UserId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Recipe>().Property(recipe => recipe.CookTime).HasConversion<string>();
        modelBuilder.Entity<RecipeDishType>().Property(dishType => dishType.Type).HasConversion<string>();

        modelBuilder.Entity<RecipeIngredient>().Property(ingredient => ingredient.Id).ValueGeneratedNever();
        modelBuilder.Entity<RecipeInstruction>().Property(instruction => instruction.Id).ValueGeneratedNever();
        modelBuilder.Entity<RecipeDishType>().Property(dishType => dishType.Id).ValueGeneratedNever();

        modelBuilder.Entity<VerificationCode>().HasOne<User>().WithMany().HasForeignKey(code => code.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<VerificationCode>().Property(code => code.Type).HasConversion<string>();


    }
}
