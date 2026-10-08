using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery;

public interface IPasswordRecoveryCodeUseCase
{
    Task Execute(RequestPasswordRecoveryJson request);
}
