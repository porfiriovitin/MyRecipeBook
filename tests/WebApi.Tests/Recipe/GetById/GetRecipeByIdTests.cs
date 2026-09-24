using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Recipe.GetById;

public class GetRecipeByIdTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe";
    private readonly UserIdentityManager _firstUser;

    public GetRecipeByIdTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _firstUser = factory.FirstUser;
    }

    [Fact]
    public async Task Success()
    {
        var recipe = _firstUser.GetRecipe();

        var response = await Get($"{REQUEST_URI}/{recipe.Id}", token: _firstUser.GetAcessToken());

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("id").GetGuid().ShouldBe(recipe.Id);
        responseData.RootElement.GetProperty("title").GetString().ShouldBe(recipe.Title);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenRecipeNotFound(string culture)
    {
        var response = await Get($"{REQUEST_URI}/{Guid.NewGuid()}", token: _firstUser.GetAcessToken(), culture: culture);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_RECIPE_NOT_FOUND", new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);

    }

}
