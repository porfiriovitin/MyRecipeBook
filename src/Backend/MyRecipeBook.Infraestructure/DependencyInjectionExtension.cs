using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infraestructure.DataAcess;
using MyRecipeBook.Infraestructure.DataAcess.Repositories;
using MyRecipeBook.Infraestructure.Identity;
using MyRecipeBook.Infraestructure.Security.PasswordHashing;
using MyRecipeBook.Infraestructure.Security.Tokens.Acess;

namespace MyRecipeBook.Infraestructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddRepositories(services);
            AddTokensHandlers(services, configuration);
            AddDbContext(services, configuration);
            AddSecurityHandlers(services);
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MyRecipeBookDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DbConnection");
                options.UseNpgsql(connectionString);
            });
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
        }

        private static void AddSecurityHandlers(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            services.AddScoped<ILoggedUser, LoggedUser>();
        }

        private static void AddTokensHandlers(this IServiceCollection services, IConfiguration configuration) 
        {
            var expirationTimeInMinutes = configuration.GetValue<uint>("Jwt:ExpirationTimeInMinutes");
            var SigningKey = configuration.GetValue<string>("Jwt:SigningKey")!;

            services.AddScoped<IAcessTokenGenerator>(provider =>
            {
                return new JwtTokenHandler(expirationTimeInMinutes, SigningKey);
            });
        }
    }
}
