using CommomTestsUtilities.Entities;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.Tokens;
using Shouldly;
using System.Net;
using System.Text.Json;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Recipe.Filter;

public class FilterRecipesTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe/filter";
    private readonly MyRecipeBookApplicationFactory _factory;
    private readonly UserIdentityManager _firstUser;

    public FilterRecipesTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _firstUser = factory.FirstUser;
    }

    [Fact]
    public async Task Success()
    {
        var response = await Post(REQUEST_URI, new RequestFilterRecipesJson(), token: _firstUser.GetAcessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        using var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe("Filtered recipes found successfully.");
        var recipes = responseData.RootElement.GetProperty("data").GetProperty("recipes");
        recipes.GetArrayLength().ShouldBe(1);
        recipes[0].GetProperty("id").GetGuid().ShouldBe(_firstUser.GetRecipe().Id);
        recipes[0].GetProperty("title").GetString().ShouldBe(_firstUser.GetRecipe().Title);
    }

    [Fact]
    public async Task ShouldReturnAllRecipes_WhenRequestIsNull()
    {
        var (userId, token) = await CreateUser();
        var recipes = Enumerable.Range(1, 8).Select(_ => RecipeBuilder.Build(userId)).ToList();
        await DbContext.Recipes.AddRangeAsync(recipes);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(null, token);

        result.OrderBy(id => id).ShouldBe(recipes.Select(recipe => recipe.Id).OrderBy(id => id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ShouldIgnoreSearchTerm_WhenNullOrWhiteSpace(string? searchTerm)
    {
        var (userId, token) = await CreateUser();
        var recipe = RecipeBuilder.Build(userId);
        await DbContext.Recipes.AddAsync(recipe);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(new RequestFilterRecipesJson { SearchTerm = searchTerm }, token);

        result.ShouldBe(new[] { recipe.Id });
    }

    [Fact]
    public async Task ShouldFilterByTitleOrIngredient()
    {
        var (userId, token) = await CreateUser();
        var titleMatch = RecipeBuilder.Build(userId);
        titleMatch.Title = "Receita com abacaxi";
        var ingredientMatch = RecipeBuilder.Build(userId);
        ingredientMatch.Title = "Sobremesa tropical";
        ingredientMatch.Ingredients = [new RecipeIngredient { Item = "Suco de abacaxi" }];
        var noMatch = RecipeBuilder.Build(userId);
        noMatch.Title = "Arroz";
        noMatch.Ingredients = [new RecipeIngredient { Item = "Sal" }];
        await DbContext.Recipes.AddRangeAsync(titleMatch, ingredientMatch, noMatch);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(new RequestFilterRecipesJson { SearchTerm = "abacaxi" }, token);

        result.OrderBy(id => id).ShouldBe(new[] { titleMatch.Id, ingredientMatch.Id }.OrderBy(id => id));
    }

    [Theory]
    [InlineData(CookTime.UpTo30Minutes)]
    [InlineData(CookTime.From30To60Minutes)]
    [InlineData(CookTime.MoreThan60Minutes)]
    public async Task ShouldFilterByCookTime(CookTime cookTime)
    {
        var (userId, token) = await CreateUser();
        var recipes = Enum.GetValues<MyRecipeBook.Domain.Enums.CookTime>().Select(time =>
        {
            var recipe = RecipeBuilder.Build(userId);
            recipe.CookTime = time;
            return recipe;
        }).ToList();
        await DbContext.Recipes.AddRangeAsync(recipes);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(new RequestFilterRecipesJson { CookTime = cookTime }, token);

        result.ShouldBe(recipes.Where(recipe => recipe.CookTime == (MyRecipeBook.Domain.Enums.CookTime)cookTime)
            .Select(recipe => recipe.Id));
    }

    [Theory]
    [InlineData(DishType.Breakfast)]
    [InlineData(DishType.Lunch)]
    [InlineData(DishType.Appetizer)]
    [InlineData(DishType.Snack)]
    [InlineData(DishType.Dessert)]
    [InlineData(DishType.Dinner)]
    [InlineData(DishType.Drink)]
    [InlineData(DishType.Salad)]
    public async Task ShouldFilterByDishType(DishType dishType)
    {
        var (userId, token) = await CreateUser();
        var recipes = Enum.GetValues<MyRecipeBook.Domain.Enums.DishType>().Select(type =>
        {
            var recipe = RecipeBuilder.Build(userId);
            recipe.DishTypes = [new RecipeDishType { Type = type, RecipeId = recipe.Id }];
            return recipe;
        }).ToList();
        await DbContext.Recipes.AddRangeAsync(recipes);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(new RequestFilterRecipesJson { DishTypes = [dishType] }, token);

        result.ShouldBe(recipes.Where(recipe => recipe.DishTypes.Any(dish => dish.Type == (MyRecipeBook.Domain.Enums.DishType)dishType))
            .Select(recipe => recipe.Id));
    }

    [Fact]
    public async Task ShouldMatchAnyDishType_WithoutDuplicateRecipes()
    {
        var (userId, token) = await CreateUser();
        var breakfast = RecipeBuilder.Build(userId);
        breakfast.DishTypes = [new RecipeDishType { Type = MyRecipeBook.Domain.Enums.DishType.Breakfast, RecipeId = breakfast.Id }];
        var snack = RecipeBuilder.Build(userId);
        snack.DishTypes = [new RecipeDishType { Type = MyRecipeBook.Domain.Enums.DishType.Snack, RecipeId = snack.Id }];
        var both = RecipeBuilder.Build(userId);
        both.DishTypes =
        [
            new RecipeDishType { Type = MyRecipeBook.Domain.Enums.DishType.Breakfast, RecipeId = both.Id },
            new RecipeDishType { Type = MyRecipeBook.Domain.Enums.DishType.Snack, RecipeId = both.Id }
        ];
        var noMatch = RecipeBuilder.Build(userId);
        noMatch.DishTypes = [new RecipeDishType { Type = MyRecipeBook.Domain.Enums.DishType.Dinner, RecipeId = noMatch.Id }];
        await DbContext.Recipes.AddRangeAsync(breakfast, snack, both, noMatch);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(new RequestFilterRecipesJson { DishTypes = [DishType.Breakfast, DishType.Snack] }, token);

        result.OrderBy(id => id).ShouldBe(new[] { breakfast.Id, snack.Id, both.Id }.OrderBy(id => id));
    }

    [Fact]
    public async Task ShouldApplyAllFiltersTogether()
    {
        var (userId, token) = await CreateUser();
        var recipes = Enumerable.Range(1, 4).Select(_ =>
        {
            var recipe = RecipeBuilder.Build(userId);
            recipe.Title = "Bolo de cenoura";
            recipe.Ingredients = [new RecipeIngredient { Item = "Farinha" }];
            recipe.CookTime = MyRecipeBook.Domain.Enums.CookTime.UpTo30Minutes;
            recipe.DishTypes = [new RecipeDishType { Type = MyRecipeBook.Domain.Enums.DishType.Snack, RecipeId = recipe.Id }];
            return recipe;
        }).ToList();
        recipes[1].Title = "Arroz";
        recipes[2].CookTime = MyRecipeBook.Domain.Enums.CookTime.MoreThan60Minutes;
        recipes[3].DishTypes = [new RecipeDishType { Type = MyRecipeBook.Domain.Enums.DishType.Dinner, RecipeId = recipes[3].Id }];
        await DbContext.Recipes.AddRangeAsync(recipes);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(new RequestFilterRecipesJson
        {
            SearchTerm = "cenoura",
            CookTime = CookTime.UpTo30Minutes,
            DishTypes = [DishType.Snack, DishType.Breakfast]
        }, token);

        result.ShouldBe(new[] { recipes[0].Id });
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoRecipesMatch()
    {
        var (userId, token) = await CreateUser();
        var recipe = RecipeBuilder.Build(userId);
        recipe.Title = "Arroz";
        recipe.Ingredients = [new RecipeIngredient { Item = "Sal" }];
        await DbContext.Recipes.AddAsync(recipe);
        await DbContext.SaveChangesAsync();

        var result = await GetRecipeIds(new RequestFilterRecipesJson { SearchTerm = "abacaxi" }, token);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenUserHasNoRecipes()
    {
        var (_, token) = await CreateUser();

        var result = await GetRecipeIds(new RequestFilterRecipesJson(), token);

        result.ShouldBeEmpty();
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

        var result = await GetRecipeIds(new RequestFilterRecipesJson(), token);

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

        var result = await GetRecipeIds(new RequestFilterRecipesJson(), token);

        result.ShouldBe(new[] { recipe.Id });
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

    private async Task<List<Guid>> GetRecipeIds(RequestFilterRecipesJson? request, string token)
    {
        var response = await Post(REQUEST_URI, request!, token: token);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        using var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe("Filtered recipes found successfully.");

        return responseData.RootElement.GetProperty("data").GetProperty("recipes")
            .EnumerateArray().Select(recipe => recipe.GetProperty("id").GetGuid()).ToList();
    }
}
