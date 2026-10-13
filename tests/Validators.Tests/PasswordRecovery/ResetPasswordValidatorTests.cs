using CommomTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Tests.PasswordRecovery;

public class ResetPasswordValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = RequestResetPasswordJsonBuilder.Build();

        var result = new ResetPasswordValidator().Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldHaveError_WhenCodeIsEmpty(string? code)
    {
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Code = code!;

        var result = new ResetPasswordValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.VALIDATION_CODE_REQUIRED);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldHaveError_WhenEmailIsEmpty(string? email)
    {
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = email!;

        var result = new ResetPasswordValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.VALIDATION_RESET_PASSWORD_EMAIL_REQUIRED);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_ShouldHaveError_WhenNewPasswordIsEmpty(string? newPassword)
    {
        var request = RequestResetPasswordJsonBuilder.Build();
        request.NewPassword = newPassword!;

        var result = new ResetPasswordValidator().Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.VALIDATION_NEW_PASSWORD_REQUIRED);
    }
}
