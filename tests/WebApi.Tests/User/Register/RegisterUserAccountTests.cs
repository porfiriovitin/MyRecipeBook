using CommomTestsUtilities.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Tests.User.Register;

public class RegisterUserAccountTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RegisterUserAccountTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();

        var response = await _client.PostAsJsonAsync("/users", request);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("Message").GetString().ShouldBe("User account registered successfully.");

        var bodyData = responseData.RootElement.GetProperty("data");
        bodyData.GetProperty("name").GetString().ShouldBe(request.Name);
        bodyData.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldBeEmpty();

    }

    [Fact]
    public async Task Validate_ShouldBeAnErrorResponse_WhenNameIsEmpty()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Name = string.Empty };

        var response = await _client.PostAsJsonAsync("/users", request);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        errors.ShouldSatisfyAllConditions( errorsList=>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString().IsNotEmpty() && error.GetString()!.Equals(ResourceMessagesException.VALIDATION_NAME_REQUIRED));
        });
    }

}
