using CommomTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User;
using MyRecipeBook.Exceptions;
using Shouldly;

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

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsEmpty()
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Name = string.Empty };
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_NAME_REQUIRED);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEmailIsEmpty()
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Email = string.Empty };
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
