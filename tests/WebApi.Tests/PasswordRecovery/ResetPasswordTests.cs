using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;

namespace WebApi.Tests.PasswordRecovery;

public class ResetPasswordTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "authentication/password-recovery/reset";
    private readonly MyRecipeBookApplicationFactory _factory;

    public ResetPasswordTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Success_ShouldUpdatePasswordAndDeleteCode_AndRejectCodeReuse()
    {
        var (user, code, oldPassword) = await CreateUserAndCode();
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = code.Code;
        request.NewPassword = "NewPassword123!";

        using var response = await Post(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await response.Content.ReadAsStringAsync()).ShouldBeEmpty();
        var savedUser = await DbContext.Users.AsNoTracking().SingleAsync(saved => saved.Id == user.Id);
        savedUser.Password.ShouldNotBe(request.NewPassword);
        savedUser.Password.ShouldNotBe(user.Password);
        using var scope = _factory.Services.CreateScope();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        hasher.VerifyPassword(request.NewPassword, savedUser.Password).ShouldBeTrue();
        hasher.VerifyPassword(oldPassword, savedUser.Password).ShouldBeFalse();
        (await DbContext.VerificationCodes.AsNoTracking().AnyAsync(saved => saved.Id == code.Id)).ShouldBeFalse();

        request.NewPassword = "AnotherPassword123!";
        using var reuseResponse = await Post(REQUEST_URI, request);

        await AssertErrorResponse(reuseResponse, "VERIFICATION_CODE_INVALID", "pt-BR");
        var passwordAfterReuse = await DbContext.Users.AsNoTracking()
            .Where(saved => saved.Id == user.Id).Select(saved => saved.Password).SingleAsync();
        passwordAfterReuse.ShouldBe(savedUser.Password);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenUserDoesNotExist(string culture)
    {
        var (user, code, _) = await CreateUserAndCode();
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = $"{Guid.NewGuid():N}@example.com";
        request.Code = code.Code;

        using var response = await Post(REQUEST_URI, request, culture: culture);

        await AssertErrorResponse(response, "VERIFICATION_CODE_INVALID", culture);
        await AssertNoChanges(user, code);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenUserIsInactive(string culture)
    {
        var (user, code, _) = await CreateUserAndCode(active: false);
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = code.Code;

        using var response = await Post(REQUEST_URI, request, culture: culture);

        await AssertErrorResponse(response, "VERIFICATION_CODE_INVALID", culture);
        await AssertNoChanges(user, code);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenCodeIsInvalid(string culture)
    {
        var (user, code, _) = await CreateUserAndCode();
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = "000000";

        using var response = await Post(REQUEST_URI, request, culture: culture);

        await AssertErrorResponse(response, "VERIFICATION_CODE_INVALID", culture);
        await AssertNoChanges(user, code);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenCodeBelongsToAnotherUser(string culture)
    {
        var (user, code, _) = await CreateUserAndCode();
        var (otherUser, otherCode, _) = await CreateUserAndCode();
        otherCode.Code = code.Code == "000001" ? "000002" : "000001";
        await DbContext.SaveChangesAsync();
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = otherCode.Code;

        using var response = await Post(REQUEST_URI, request, culture: culture);

        await AssertErrorResponse(response, "VERIFICATION_CODE_INVALID", culture);
        await AssertNoChanges(user, code);
        await AssertNoChanges(otherUser, otherCode);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenCodeIsExpired(string culture)
    {
        var (user, code, _) = await CreateUserAndCode(ageInMinutes: 11);
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = code.Code;

        using var response = await Post(REQUEST_URI, request, culture: culture);

        await AssertErrorResponse(response, "VERIFICATION_CODE_EXPIRED", culture);
        await AssertNoChanges(user, code);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenNewPasswordIsEmpty(string culture)
    {
        var (user, code, _) = await CreateUserAndCode();
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = code.Code;
        request.NewPassword = string.Empty;

        using var response = await Post(REQUEST_URI, request, culture: culture);

        await AssertErrorResponse(response, "VALIDATION_NEW_PASSWORD_REQUIRED", culture);
        await AssertNoChanges(user, code);
    }

    private async Task<(MyRecipeBook.Domain.Entities.User User, VerificationCode Code, string Password)>
        CreateUserAndCode(bool active = true, int ageInMinutes = 0)
    {
        var (user, password) = UserBuilder.Build();
        user.Email = $"{user.Id:N}@example.com";
        user.Active = active;
        using var scope = _factory.Services.CreateScope();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        user.Password = hasher.HashPassword(password);
        var code = VerificationCodeBuilder.Build(user.Id);
        code.CreatedAt = DateTime.UtcNow.AddMinutes(-ageInMinutes);

        await DbContext.Users.AddAsync(user);
        await DbContext.VerificationCodes.AddAsync(code);
        await DbContext.SaveChangesAsync();

        return (user, code, password);
    }

    private async Task AssertNoChanges(MyRecipeBook.Domain.Entities.User user, VerificationCode code)
    {
        var savedUser = await DbContext.Users.AsNoTracking().SingleAsync(saved => saved.Id == user.Id);
        savedUser.Password.ShouldBe(user.Password);
        var savedCode = await DbContext.VerificationCodes.AsNoTracking().SingleAsync(saved => saved.Id == code.Id);
        savedCode.Code.ShouldBe(code.Code);
        savedCode.UserId.ShouldBe(code.UserId);
        savedCode.Type.ShouldBe(code.Type);
        savedCode.CreatedAt.ShouldBe(code.CreatedAt, TimeSpan.FromMilliseconds(1));
    }

    private static async Task AssertErrorResponse(HttpResponseMessage response, string resourceKey, string culture)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await using var body = await response.Content.ReadAsStreamAsync();
        using var data = await JsonDocument.ParseAsync(body);
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString(resourceKey, new CultureInfo(culture));
        expectedMessage.ShouldNotBeNullOrEmpty();
        data.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Error));
        data.RootElement.GetProperty("message").GetString().ShouldBe(expectedMessage);
    }
}
