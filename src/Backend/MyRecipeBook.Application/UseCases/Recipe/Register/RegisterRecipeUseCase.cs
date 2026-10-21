using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using FluentValidation.Results;
using Mapster;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Cache;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe;

public class RegisterRecipeUseCase : IRegisterRecipeUseCase
{
    private readonly IRecipeWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipesCache _cache;
    private readonly IStorageService _storageService;

    public RegisterRecipeUseCase(IRecipeWriteOnlyRepository recipeWriteOnlyRepository, IUnitOfWork unitOfWork, ILoggedUser loggedUser, IRecipesCache cache, IStorageService storageService)
    {
        _repository = recipeWriteOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
        _cache = cache;
        _storageService = storageService;
    }

    public async Task<ResponseRegisteredRecipeJson> Execute(RequestRecipeJson request, Stream? recipeIlustration)
    {
        await ValidateAndThrowOnFailures(request);

        var recipe = request.Adapt<Domain.Entities.Recipe>();
        recipe.UserId = _loggedUser.GetUserId();
                
        await _repository.AddAsync(recipe);

        await _unitOfWork.Commit();

        if (recipeIlustration is not null)
        {
            var contentType = recipeIlustration.DetectImageContentType();

            if (contentType.IsEmpty())
                throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);

            await _storageService.UploadIllustration(recipe, recipeIlustration, contentType);
        }

        _cache.RemoveRecent(_loggedUser.GetUserId());

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
