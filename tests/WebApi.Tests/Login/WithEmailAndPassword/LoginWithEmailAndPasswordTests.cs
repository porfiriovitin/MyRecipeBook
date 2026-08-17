using CommomTestsUtilities.Requests;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/authentication";
    private readonly UserIdentityManager _firstUser;

    public LoginWithEmailAndPasswordTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _firstUser = factory.FirstUser ?? throw new InvalidOperationException("First user is not initialized.");
    }

    [Fact]
    public async Task Sucess()
    {
        var request = new RequestLoginJson
        {
            Email = _firstUser.GetEmail(),
            Password = _firstUser.GetPassword()
        };

        var response = await Post(REQUEST_URI, request);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe("Login sucessfully");

        var bodyData = responseData.RootElement.GetProperty("data");
        bodyData.GetProperty("name").GetString().ShouldBe(_firstUser.GetName());
        bodyData.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldNotBeEmpty();

    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task ShoudThrowException_WhenUserDontExists(string culture) 
    {
        var request = RequestLoginJsonBuilder.Build();

        var response =await Post(REQUEST_URI, request, culture: culture);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_LOGIN_INVALID", new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);
    }

}
