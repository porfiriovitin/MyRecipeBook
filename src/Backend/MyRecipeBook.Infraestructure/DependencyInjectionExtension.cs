using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Infraestructure.Security.PasswordHashing;

namespace MyRecipeBook.Infraestructure
{
    public class DependencyInjectionExtension
    {
        public static void AddInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        }
    }
}
