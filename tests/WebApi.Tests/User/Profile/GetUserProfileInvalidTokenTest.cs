using Shouldly;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.Profile;

public class GetUserProfileInvalidTokenTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "/user";
    private readonly string _tokenUserNotExistsDatabase;

    public GetUserProfileInvalidTokenTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistsDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var response = await Get(REQUEST_URI, _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var response = await Get(REQUEST_URI, token: string.Empty);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_Not_Found()
    {
        var response = await Get(REQUEST_URI, token: _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }
}
