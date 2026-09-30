using CommomTestsUtilities.Entities;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Domain.Security.Tokens;
using Shouldly;
using System.Net;
using System.Text.Json;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Recipe.Recent;

public class GetRecentRecipesTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe/recent";
    private readonly MyRecipeBookApplicationFactory _factory;
    private readonly UserIdentityManager _firstUser;

    public GetRecentRecipesTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _firstUser = factory.FirstUser;
    }

    [Fact]
    public async Task Success()
    {
        var response = await Get(REQUEST_URI, token: _firstUser.GetAcessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        using var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe("Recent recipes found successfully.");
        var recipes = responseData.RootElement.GetProperty("data").GetProperty("recipes");
        recipes.GetArrayLength().ShouldBe(1);
        recipes[0].GetProperty("id").GetGuid().ShouldBe(_firstUser.GetRecipe().Id);
        recipes[0].GetProperty("title").GetString().ShouldBe(_firstUser.GetRecipe().Title);
    }

    [Fact]
    public async Task ShouldReturnSixMostRecentRecipes_InDescendingCreationOrder()
    {
        var (userId, token) = await CreateUser();
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var recipes = Enumerable.Range(1, 8).Select(index =>
        {
            var recipe = RecipeBuilder.Build(userId);
            recipe.CreatedAt = createdAt.AddDays(index);
            return recipe;
        }).ToList();
        await DbContext.Recipes.AddRangeAsync(recipes);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(token);

        result.ShouldBe(recipes.OrderByDescending(recipe => recipe.CreatedAt).Take(6).Select(recipe => recipe.Id));
    }

    [Fact]
    public async Task ShouldExcludeInactiveRecipes()
    {
        var (userId, token) = await CreateUser();
        var activeRecipe = RecipeBuilder.Build(userId);
        var inactiveRecipe = RecipeBuilder.Build(userId);
        inactiveRecipe.Active = false;
        await DbContext.Recipes.AddRangeAsync(activeRecipe, inactiveRecipe);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(token);

        result.ShouldBe(new[] { activeRecipe.Id });
    }

    [Fact]
    public async Task ShouldExcludeRecipesFromAnotherUser()
    {
        var (userId, token) = await CreateUser();
        var recipe = RecipeBuilder.Build(userId);
        var (otherUserId, _) = await CreateUser();
        var otherUserRecipe = RecipeBuilder.Build(otherUserId);
        await DbContext.Recipes.AddRangeAsync(recipe, otherUserRecipe);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(token);

        result.ShouldBe(new[] { recipe.Id });
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenUserHasNoRecipes()
    {
        var (_, token) = await CreateUser();

        var result = await GetRecipeIds(token);

        result.ShouldBeEmpty();
    }

    private async Task<(Guid userId, string token)> CreateUser()
    {
        var (user, _) = UserBuilder.Build();
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        using var scope = _factory.Services.CreateScope();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<IAcessTokenGenerator>();
        return (user.Id, tokenGenerator.Generate(user));
    }

    private async Task<List<Guid>> GetRecipeIds(string token)
    {
        var response = await Get(REQUEST_URI, token: token);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        using var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));

        return responseData.RootElement.GetProperty("data").GetProperty("recipes")
            .EnumerateArray().Select(recipe => recipe.GetProperty("id").GetGuid()).ToList();
    }
}
