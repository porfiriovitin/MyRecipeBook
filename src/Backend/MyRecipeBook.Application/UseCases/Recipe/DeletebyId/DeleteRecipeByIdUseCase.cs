using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Cache;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.DeletebyId;

public class DeleteRecipeByIdUseCase : IDeleteRecipebyIdUseCase
{
    private readonly IRecipeWriteOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipesCache _cache;

    public DeleteRecipeByIdUseCase(IRecipeWriteOnlyRepository repository, ILoggedUser loggedUser, IRecipesCache cache)
    {
        _repository = repository;
        _loggedUser = loggedUser;
        _cache = cache;
    }

    public async Task Execute(Guid id)
    {
        var deleted = await _repository.DeleteById(id, _loggedUser.GetUserId());

        if (deleted == false)
            throw new NotFoundException(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);

        _cache.RemoveRecent(_loggedUser.GetUserId());
    }
}
