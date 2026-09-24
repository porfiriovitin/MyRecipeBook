using WebApi.Tests.Resources;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using CommomTestsUtilities.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infraestructure.DataAcess;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Security.PasswordHashing;

namespace WebApi.Tests;

public class MyRecipeBookApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public UserIdentityManager FirstUser { get; private set; } = default!;

    public string TOKEN_USER_NOT_FOUND_IN_DATABASE { get; private set; } = string.Empty;

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
        var acessTokenGenerator = scope.ServiceProvider.GetRequiredService<IAcessTokenGenerator>();

        var (user, password) = UserBuilder.Build();
        user.Password = passwordHasher.HashPassword(password);

        var recipe = RecipeBuilder.Build(user.Id);

        await dbContext.Users.AddAsync(user);
        await dbContext.Recipes.AddAsync(recipe);

        await dbContext.SaveChangesAsync();

        var firstUserAcessToken = acessTokenGenerator.Generate(user);

        FirstUser = new UserIdentityManager(user, recipe, password, firstUserAcessToken);

        TOKEN_USER_NOT_FOUND_IN_DATABASE = acessTokenGenerator.Generate(new MyRecipeBook.Domain.Entities.User());
    }

    Task IAsyncLifetime.DisposeAsync() => _postgreSqlContainer.StopAsync();

}
