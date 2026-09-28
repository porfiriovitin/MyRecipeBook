using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Recipe.DeleteById;

public class DeleteRecipeByIdTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe";
    private readonly UserIdentityManager _firstUser;

    public DeleteRecipeByIdTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _firstUser = factory.FirstUser;
    }

    [Fact]
    public async Task Success()
    {
        var recipe = _firstUser.GetRecipe();

        var response = await Delete($"{REQUEST_URI}/{recipe.Id}", token: _firstUser.GetAcessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await DbContext.Recipes.AnyAsync(item => item.Id == recipe.Id)).ShouldBeFalse();
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenRecipeNotFound(string culture)
    {
        var response = await Delete($"{REQUEST_URI}/{Guid.NewGuid()}", token: _firstUser.GetAcessToken(), culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        using var responseData = await JsonDocument.ParseAsync(responseBody);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_RECIPE_NOT_FOUND", new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);
    }
}
