using CommomTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User;

namespace Validators.Tests.User.Register;

public class RegisterUserAccountValidatorTests
{
    [Fact]
    public void Sucess()
    {
        /// :: Arrange.
        var request = RequestRegisterUserAccountJsonBuilder.Build();
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        Assert.True(result.IsValid);
    }
}
