using MyRecipeBook.Application.UseCases.User;
using MyRecipeBook.Communication.Requests;

namespace Validators.Tests.User.Register;

public class RegisterUserAccountValidatorTests
{
    [Fact]
    public void Sucess()
    {
        /// :: Arrange.
        var request = new RequestRegisterUserAccountJson(Name: "John Doe", Email: "john.doe@example.com", Password: "SecurePassword123!");
        var validator = new RegisterUserAccountValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        Assert.True(result.IsValid);
    }
}
