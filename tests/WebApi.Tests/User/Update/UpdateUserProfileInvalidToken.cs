using CommomTestsUtilities.Requests;
using Shouldly;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.Update;

public class UpdateUserProfileInvalidToken : BaseIntegrationTest
{
    private const string REQUEST_URI = "user/profile";
    private readonly string _tokenUserNotExistsDatabase;

    public UpdateUserProfileInvalidToken(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistsDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await Put(REQUEST_URI, request, _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await Put(REQUEST_URI, request, token: string.Empty);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_With_User_Not_Found()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await Put(REQUEST_URI, request, token: _tokenUserNotExistsDatabase);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }
}
