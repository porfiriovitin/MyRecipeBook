namespace MyRecipeBook.Application.UseCases.Recipe.Delete;

public interface IDeleteRecipebyIdUseCase
{
    Task Execute(Guid id);
}
