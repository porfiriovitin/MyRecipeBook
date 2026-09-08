using CommomTestsUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;
using Org.BouncyCastle.Asn1.Ocsp;
using Shouldly;
using System.Globalization;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.Update;

public class UpdateUserTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "user/profile";
    private readonly UserIdentityManager _user1;

    public UpdateUserTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.FirstUser;
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await Put(REQUEST_URI, request, token: _user1.GetAcessToken());
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var userExists = await DbContext.Users.AnyAsync(u => u.Active && u.Id == _user1.GetId() && u.Name == request.Name && u.Email == request.Email);

        userExists.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenNameIsEmpty(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var response = await Put(REQUEST_URI, request, token: _user1.GetAcessToken(), culture: culture);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));

        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe(expectedErrorMessage);
    }

}
