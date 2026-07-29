using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using System.Runtime.CompilerServices;

[assembly:  InternalsVisibleTo("WebApi.Tests")]
namespace MyRecipeBook.Infraestructure.DataAcess;

internal class MyRecipeBookDbContext : DbContext
{
    public MyRecipeBookDbContext(DbContextOptions<MyRecipeBookDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}
