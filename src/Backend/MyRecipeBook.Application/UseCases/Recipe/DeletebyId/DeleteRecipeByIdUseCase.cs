using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.DeletebyId;

public class DeleteRecipeByIdUseCase : IDeleteRecipebyIdUseCase
{
    private readonly IRecipeWriteOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;

    public DeleteRecipeByIdUseCase(IRecipeWriteOnlyRepository repository, ILoggedUser loggedUser)
    {
        _repository = repository;
        _loggedUser = loggedUser;
    }

    public async Task Execute(Guid id)
    {
        var deleted = await _repository.DeleteById(id, _loggedUser.GetUserId());

        if (deleted == false)
            throw new NotFoundException(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
    }
}
