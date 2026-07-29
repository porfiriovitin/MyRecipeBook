using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace WebApi.Tests;

public class MyRecipeBookApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
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
    }

    Task IAsyncLifetime.DisposeAsync() => _postgreSqlContainer.StopAsync();
    
}
