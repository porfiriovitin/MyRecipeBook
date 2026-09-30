using CommomTestsUtilities.Requests;
using Shouldly;
using System.Net;

namespace WebApi.Tests.Recipe.UpdateById;

public class UpdateRecipeByIdInvalidTokenTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "recipe";
    private readonly string _tokenUserNotExistsDatabase;

    public UpdateRecipeByIdInvalidTokenTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistsDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenAccessTokenIsInvalid()
    {
        var response = await Put($"{REQUEST_URI}/{Guid.NewGuid()}", RequestRecipeJsonBuilder.Build(), token: "invalid_token");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenAccessTokenIsMissing()
    {
        var response = await Put($"{REQUEST_URI}/{Guid.NewGuid()}", RequestRecipeJsonBuilder.Build(), token: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenUserFromAccessTokenDoesNotExist()
    {
        var response = await Put($"{REQUEST_URI}/{Guid.NewGuid()}", RequestRecipeJsonBuilder.Build(), token: _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
