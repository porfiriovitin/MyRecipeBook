using CommomTestsUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordTests : IClassFixture<MyRecipeBookApplicationFactory>
{
    private const string REQUEST_URI = "/authentication";
    private readonly HttpClient _httpClient;
    private readonly UserIdentityManager _firstUser;

    public LoginWithEmailAndPasswordTests(MyRecipeBookApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
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

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe("Login sucessfully");

        var bodyData = responseData.RootElement.GetProperty("data");
        bodyData.GetProperty("name").GetString().ShouldBe(_firstUser.GetName());
        bodyData.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldBeEmpty();

    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task ShoudThrowException_WhenUserDontExists(string culture) 
    {
        var request = RequestLoginJsonBuilder.Build();

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var response =await _httpClient.PostAsJsonAsync(REQUEST_URI, request);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_LOGIN_INVALID", new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);
    }

}
