using CommomTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Diagnostics.CodeAnalysis;

namespace Validators.Tests.User.Register;

public class RegisterUserAccountValidatorTests
{
    [Fact]
    public void Success()
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build();
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Intentional because is a unit test")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string name)
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Name = name };
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_NAME_REQUIRED);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Intentional because is a unit test")]
    public void Validate_ShouldHaveError_WhenEmailIsEmpty(string email)
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Email = email };
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_EMAIL_REQUIRED);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEmailIsInvalid()
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Email = "invalid-email" };
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_EMAIL_INVALID);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordIsEmpty()
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Password = string.Empty };
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordIsShorterThanSixCharacters()
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Password = "12345" };
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_PASSWORD_MIN_LENGTH);
    }
}
