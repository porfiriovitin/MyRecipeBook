using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Infraestructure.DataAcess.Repositories;
using MyRecipeBook.Infraestructure.Security.PasswordHashing;

namespace MyRecipeBook.Infraestructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        }
    }
}
