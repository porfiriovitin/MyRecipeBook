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

namespace WebApi.Tests.Recipe.Register;

public class RegisterRecipeTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/recipe";
    private readonly UserIdentityManager _user;

    public RegisterRecipeTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user = factory.FirstUser;
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, token: _user.GetAcessToken());
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe("Recipe registered successfully.");

        var bodyData = responseData.RootElement.GetProperty("data");
        bodyData.GetProperty("title").GetString().ShouldBe(request.Title);

        var recipeId = bodyData.GetProperty("id").GetGuid();
        var recipe = await DbContext.Recipes.SingleAsync(recipe => recipe.Id == recipeId);

        recipe.Title.ShouldBe(request.Title);
        recipe.UserId.ShouldBe(_user.GetId());
        recipe.CookTime.ShouldBe((MyRecipeBook.Domain.Enums.CookTime)request.CookTime);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenTitleIsEmpty(string culture)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var response = await Post(REQUEST_URI, request, token: _user.GetAcessToken(), culture: culture);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_RECIPE_TITLE_REQUIRED", new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);

        var recipeExists = await DbContext.Recipes.AnyAsync(recipe => recipe.UserId == _user.GetId() && recipe.Title.Equals(request.Title));
        recipeExists.ShouldBeFalse();
    }
}
