using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Enums;
using Shouldly;
using System.Net;
using WebApi.Tests.Resources;

namespace WebApi.Tests.PasswordRecovery;

public class PasswordRecoveryCodeTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "authentication/password-recovery";
    private readonly UserIdentityManager _firstUser;

    public PasswordRecoveryCodeTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _firstUser = factory.FirstUser;
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestPasswordRecoveryJsonBuilder.Build();
        request.Email = _firstUser.GetEmail();
        var existingCodeIds = await DbContext.VerificationCodes.Select(code => code.Id).ToListAsync();

        using var response = await Post(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        (await response.Content.ReadAsStringAsync()).ShouldBeEmpty();

        var newCodes = await DbContext.VerificationCodes.AsNoTracking()
            .Where(code => !existingCodeIds.Contains(code.Id)).ToListAsync();

        newCodes.Count.ShouldBe(1);
        var savedCode = newCodes.Single();
        savedCode.UserId.ShouldBe(_firstUser.GetId());
        savedCode.Type.ShouldBe(VerificationCodeType.PasswordRecovery);
        savedCode.Code.ShouldMatch("^[0-9]{6}$");
        int.Parse(savedCode.Code).ShouldBeInRange(1, 999999);
    }

    [Fact]
    public async Task Success_WhenUserDoesNotExist()
    {
        var request = RequestPasswordRecoveryJsonBuilder.Build();
        request.Email = $"{Guid.NewGuid():N}@example.com";
        var existingCodeCount = await DbContext.VerificationCodes.CountAsync();

        using var response = await Post(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        (await response.Content.ReadAsStringAsync()).ShouldBeEmpty();
        (await DbContext.VerificationCodes.CountAsync()).ShouldBe(existingCodeCount);
    }

    [Fact]
    public async Task Success_WhenUserIsInactive()
    {
        var (user, _) = UserBuilder.Build();
        user.Email = $"{user.Id:N}@example.com";
        user.Active = false;
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        var request = RequestPasswordRecoveryJsonBuilder.Build();
        request.Email = user.Email;
        var existingCodeCount = await DbContext.VerificationCodes.CountAsync();

        using var response = await Post(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        (await response.Content.ReadAsStringAsync()).ShouldBeEmpty();
        (await DbContext.VerificationCodes.CountAsync()).ShouldBe(existingCodeCount);
    }
}
