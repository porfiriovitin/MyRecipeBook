using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.ChangeProfilePicture;

public class ChangeProfilePictureUseCase : IChangeProfilePictureUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IStorageService _storageService;

    public ChangeProfilePictureUseCase(ILoggedUser loggedUser, IStorageService storageService)
    {
        _loggedUser = loggedUser;
        _storageService = storageService;   
    }

    public async Task Execute(Stream profilePicture)
    {
        var contentType = profilePicture.DetectImageContentType();
        if (contentType.IsEmpty())
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);

        var loggedUser = await _loggedUser.Get();

        await _storageService.UploadProfilePicture(loggedUser, profilePicture, contentType);
    }
}
