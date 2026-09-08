using MyRecipeBook.Communication.Requests;
using Shouldly;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.ChangePassword;

public class ChangePasswordInvalidTokenTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "user/password";
    private readonly string _tokenUserNotExistsDatabase;

    public ChangePasswordInvalidTokenTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistsDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = new RequestChangePasswordJson();
        
        var response = await Put(REQUEST_URI, request, _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = new RequestChangePasswordJson();

        var response = await Put(REQUEST_URI, request, token: string.Empty);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_Not_Found()
    {
        var request = new RequestChangePasswordJson();

        var response = await Put(REQUEST_URI, request, token: _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

}
