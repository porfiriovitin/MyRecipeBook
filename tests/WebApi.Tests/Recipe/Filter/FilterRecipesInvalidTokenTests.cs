using CommomTestsUtilities.Requests;
using Shouldly;
using System.Net;

namespace WebApi.Tests.Recipe.Filter;

public class FilterRecipesInvalidTokenTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe/filter";
    private readonly string _tokenUserNotExistsDatabase;

    public FilterRecipesInvalidTokenTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistsDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenAccessTokenIsInvalid()
    {
        var request = RequestFilterRecipesJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, token: "invalid_token");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenAccessTokenIsMissing()
    {
        var request = RequestFilterRecipesJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, token: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenUserFromAccessTokenDoesNotExist()
    {
        var request = RequestFilterRecipesJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, token: _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
