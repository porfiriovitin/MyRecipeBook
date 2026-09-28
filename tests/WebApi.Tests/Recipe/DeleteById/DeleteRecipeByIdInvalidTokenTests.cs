using Shouldly;
using System.Net;

namespace WebApi.Tests.Recipe.DeleteById;

public class DeleteRecipeByIdInvalidTokenTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe";
    private readonly string _tokenUserNotExistsDatabase;

    public DeleteRecipeByIdInvalidTokenTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistsDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenAccessTokenIsInvalid()
    {
        var response = await Delete($"{REQUEST_URI}/{Guid.NewGuid()}", token: "invalid_token");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenAccessTokenIsMissing()
    {
        var response = await Delete($"{REQUEST_URI}/{Guid.NewGuid()}", token: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenUserFromAccessTokenDoesNotExist()
    {
        var response = await Delete($"{REQUEST_URI}/{Guid.NewGuid()}", token: _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
