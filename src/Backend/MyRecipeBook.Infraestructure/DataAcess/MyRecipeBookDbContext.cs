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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipe>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(recipe => recipe.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
