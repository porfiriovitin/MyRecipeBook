using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Recipe.UpdateById;

public class UpdateRecipeByIdTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe";
    private readonly UserIdentityManager _firstUser;

    public UpdateRecipeByIdTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _firstUser = factory.FirstUser;
    }

    [Fact]
    public async Task Success()
    {
        var recipeId = _firstUser.GetRecipe().Id;
        var request = RequestRecipeJsonBuilder.Build();

        var response = await Put($"{REQUEST_URI}/{recipeId}", request, token: _firstUser.GetAcessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var recipe = await DbContext.Recipes.AsNoTracking().Include(item => item.Ingredients).Include(item => item.Instructions).Include(item => item.DishTypes).SingleAsync(item => item.Id == recipeId);

        recipe.Id.ShouldBe(recipeId);
        recipe.UserId.ShouldBe(_firstUser.GetId());
        recipe.Title.ShouldBe(request.Title);
        recipe.CookTime.ShouldBe((MyRecipeBook.Domain.Enums.CookTime)request.CookTime);
        recipe.Ingredients.Select(item => item.Item).ShouldBe(request.Ingredients.Select(item => item.Item));
        recipe.Instructions.OrderBy(item => item.Order).Select(item => (item.Order, item.Description)).ShouldBe(request.Instructions.OrderBy(item => item.Order).Select(item => (item.Order, item.Description)));
        recipe.DishTypes.Select(item => item.Type).ShouldBe(request.DishTypes.Select(item => (MyRecipeBook.Domain.Enums.DishType)item));
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenTitleIsEmpty(string culture)
    {
        var recipeId = _firstUser.GetRecipe().Id;
        var originalTitle = await DbContext.Recipes.AsNoTracking().Where(item => item.Id == recipeId).Select(item => item.Title).SingleAsync();
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var response = await Put($"{REQUEST_URI}/{recipeId}", request, token: _firstUser.GetAcessToken(), culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertErrorResponse(response, "VALIDATION_RECIPE_TITLE_REQUIRED", culture);

        var persistedTitle = await DbContext.Recipes.AsNoTracking().Where(item => item.Id == recipeId).Select(item => item.Title).SingleAsync();
        persistedTitle.ShouldBe(originalTitle);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenRecipeNotFound(string culture)
    {
        var response = await Put($"{REQUEST_URI}/{Guid.NewGuid()}", RequestRecipeJsonBuilder.Build(), token: _firstUser.GetAcessToken(), culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertErrorResponse(response, "VALIDATION_RECIPE_NOT_FOUND", culture);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenRecipeBelongsToAnotherUser(string culture)
    {
        var (otherUser, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(otherUser.Id);
        var originalTitle = recipe.Title;
        await DbContext.Users.AddAsync(otherUser);
        await DbContext.Recipes.AddAsync(recipe);
        await DbContext.SaveChangesAsync();

        var response = await Put($"{REQUEST_URI}/{recipe.Id}", RequestRecipeJsonBuilder.Build(), token: _firstUser.GetAcessToken(), culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await AssertErrorResponse(response, "VALIDATION_RECIPE_NOT_FOUND", culture);

        var persistedTitle = await DbContext.Recipes.AsNoTracking().Where(item => item.Id == recipe.Id).Select(item => item.Title).SingleAsync();
        persistedTitle.ShouldBe(originalTitle);
    }

    private static async Task AssertErrorResponse(HttpResponseMessage response, string resourceKey, string culture)
    {
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        using var responseData = await JsonDocument.ParseAsync(responseBody);
        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString(resourceKey, new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);
    }
}
