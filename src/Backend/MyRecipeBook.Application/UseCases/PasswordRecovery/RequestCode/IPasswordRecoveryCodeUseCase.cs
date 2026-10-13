using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.RequestCode;

public interface IPasswordRecoveryCodeUseCase
{
    Task Execute(RequestPasswordRecoveryJson request);
}
