using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.UpdatebyId;

public class UpdateRecipebyIdUseCase : IUpdateRecipeByIdUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRecipebyIdUseCase(ILoggedUser loggedUser, IRecipeUpdateOnlyRepository repository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _unitOfWork = unitOfWork;

    }

    public async Task Execute(Guid RecipeId, RequestRecipeJson request)
    {
        ValidateAndThrownOnFailures(request);

        var recipe = await _repository.GetByIdAsync(RecipeId, _loggedUser.GetUserId());

        if (recipe is null)
            throw new NotFoundException(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);

        request.Adapt(recipe);

        await _unitOfWork.Commit();
    }


    private static void ValidateAndThrownOnFailures(RequestRecipeJson request)
    {
        var result = new RequestRecipeValidator().Validate(request);

        if (result.IsValid is false)
            throw new ErrorOnValidationException([.. result.Errors.Select(x => x.ErrorMessage)]);
    }

}