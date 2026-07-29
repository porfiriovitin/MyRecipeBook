using CommomTestsUtilities.Requests;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Infraestructure.DataAcess;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InlineData;

namespace WebApi.Tests.User.Register;

public class RegisterUserAccountTests : IClassFixture<MyRecipeBookApplicationFactory>
{
    private const string REQUEST_URI = "/user";
    private readonly HttpClient _client;
    private readonly MyRecipeBookDbContext _dbContext;

    public RegisterUserAccountTests(MyRecipeBookApplicationFactory factory)
    {
        _client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();

        var response = await _client.PostAsJsonAsync(REQUEST_URI, request);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe("User account registered successfully.");

        var bodyData = responseData.RootElement.GetProperty("data");
        bodyData.GetProperty("name").GetString().ShouldBe(request.Name);
        bodyData.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldBeEmpty();

        var userExists = _dbContext.Users.Any(user => user.Active && user.Name.Equals(request.Name) && user.Email.Equals(request.Email));
        userExists.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenNameIsEmpty(string culture)
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Name = string.Empty };

        _client.DefaultRequestHeaders.AcceptLanguage.Clear();
        _client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var response = await _client.PostAsJsonAsync(REQUEST_URI, request);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);

        var userExists = _dbContext.Users.Any(user => user.Active && user.Name.Equals(request.Name) && user.Email.Equals(request.Email));
        userExists.ShouldBeFalse();
    }
}
