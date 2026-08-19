using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordUseCase(ILoggedUser loggedUser, IPasswordHasher passwordHasher)
    {
        _loggedUser = loggedUser;
        _passwordHasher = passwordHasher;
    }

    public async Task Execute(RequestPasswordJson request)
    {
        var loggedUser = await _loggedUser.Get();

        Validate(request, loggedUser);

    }

    private void Validate(RequestPasswordJson request, Domain.Entities.User loggedUser)
    {
        var result = new ChangePasswordValidator().Validate(request);

        if(_passwordHasher.VerifyPassword(request.CurrentPassword, loggedUser.Password) == false)
            throw new ErrorOnValidationException(new List<string> { "Current password is incorrect." });

        if (!result.IsValid)
            throw new ErrorOnValidationException([.. result.Errors.Select(error => error.ErrorMessage)]);
    }
}
