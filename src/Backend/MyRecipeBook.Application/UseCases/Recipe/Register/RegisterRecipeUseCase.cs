using Mapster;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe;

internal class RegisterRecipeUseCase : IRegisterRecipeUseCase
{
    private readonly IRecipeWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public RegisterRecipeUseCase(IRecipeWriteOnlyRepository recipeWriteOnlyRepository, IUnitOfWork unitOfWork, ILoggedUser loggedUser)
    {
        _repository = recipeWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseRegisteredRecipeJson> Execute(RequestRecipeJson request)
    {
        await ValidateAndThrowOnFailures(request);

        var recipe = request.Adapt<Domain.Entities.Recipe>();
        recipe.UserId = _loggedUser.GetUserId();

        await _repository.AddAsync(recipe);

        await _unitOfWork.Commit();

        return new ResponseRegisteredRecipeJson
        {
            Id = recipe.Id,
            Title = recipe.Title
        };
    }

    private static async Task ValidateAndThrowOnFailures(RequestRecipeJson request)
    {
        var result = new RequestRecipeValidator().Validate(request);

        if (!result.IsValid)
            throw new ErrorOnValidationException([.. result.Errors.Select(error => error.ErrorMessage)]);
    }
}
