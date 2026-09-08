using CommomTestsUtilities.Requests;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.ChangePassword;

public class ChangePasswordTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "user/password";

    private readonly UserIdentityManager _user1;

    public ChangePasswordTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.FirstUser;
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        request.CurrentPassword = _user1.GetPassword();

        var response = await Put(REQUEST_URI, request, token: _user1.GetAcessToken());

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenNewPasswordIsEmpty(string culture)
    {
        var request = new RequestChangePasswordJson
        {
            CurrentPassword = _user1.GetPassword(),
            NewPassword = string.Empty
        };

        var response = await Put(REQUEST_URI, request, token: _user1.GetAcessToken(), culture: culture);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_PASSWORD_REQUIRED", new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);
    }

}
