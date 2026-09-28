using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.Recipe.UpdatebyId;

public interface IUpdateRecipeByIdUseCase
{
    Task Execute(Guid id, RequestRecipeJson request);
}
