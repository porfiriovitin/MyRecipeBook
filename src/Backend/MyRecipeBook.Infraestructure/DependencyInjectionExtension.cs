using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Infraestructure.DataAcess;
using MyRecipeBook.Infraestructure.DataAcess.Repositories;
using MyRecipeBook.Infraestructure.Security.PasswordHashing;

namespace MyRecipeBook.Infraestructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<MyRecipeBookDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DbConnection");
                options.UseNpgsql(connectionString);
            });
        }
    }
}
