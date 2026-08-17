using Shouldly;
using System.Net;
using System.Text.Json;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.Profile;

public class GetUserProfileTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "/user";
    private readonly UserIdentityManager _user1;

    public GetUserProfileTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.FirstUser!;
    }

    [Fact]
    public async Task Sucess()
    {
        var response = await Get(REQUEST_URI, token: _user1.GetAcessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var bodyData = responseData.RootElement.GetProperty("data");
        bodyData.GetProperty("name").GetString().ShouldBe(_user1.GetName());
        bodyData.GetProperty("email").GetString().ShouldBe(_user1.GetEmail());
    }

}
