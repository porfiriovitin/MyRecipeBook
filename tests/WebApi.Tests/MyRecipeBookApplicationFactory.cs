using CommomTestsUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Infraestructure.DataAcess;
using Testcontainers.PostgreSql;
using WebApi.Tests.Resources;

namespace WebApi.Tests;

public class MyRecipeBookApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public UserIdentityManager? FirstUser { get; private set; }

    private readonly PostgreSqlContainer _postgreSqlContainer;

    public MyRecipeBookApplicationFactory()
    {
        _postgreSqlContainer = new PostgreSqlBuilder("postgres:16").WithDatabase("postgres").Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests").ConfigureAppConfiguration((_, configuration) =>
        {
            var parameters = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DbConnection"] = _postgreSqlContainer.GetConnectionString()
            };

            configuration.AddInMemoryCollection(parameters);
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var (user, password) = UserBuilder.Build();

        user.Password = passwordHasher.HashPassword(password);

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        FirstUser = new UserIdentityManager(user, password);
    }

    Task IAsyncLifetime.DisposeAsync() => _postgreSqlContainer.StopAsync();
    
}
