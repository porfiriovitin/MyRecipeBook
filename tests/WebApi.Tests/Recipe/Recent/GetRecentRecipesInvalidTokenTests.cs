using Shouldly;
using System.Net;

namespace WebApi.Tests.Recipe.Recent;

public class GetRecentRecipesInvalidTokenTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe/recent";
    private readonly string _tokenUserNotExistsDatabase;

    public GetRecentRecipesInvalidTokenTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistsDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenAccessTokenIsInvalid()
    {
        var response = await Get(REQUEST_URI, token: "invalid_token");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenAccessTokenIsMissing()
    {
        var response = await Get(REQUEST_URI, token: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenUserFromAccessTokenDoesNotExist()
    {
        var response = await Get(REQUEST_URI, token: _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
