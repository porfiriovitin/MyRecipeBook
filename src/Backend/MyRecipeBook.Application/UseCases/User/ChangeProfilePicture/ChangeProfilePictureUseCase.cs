using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.ChangeProfilePicture;

public class ChangeProfilePictureUseCase : IChangeProfilePictureUseCase
{
    private readonly ILoggedUser _loggedUser;

    public ChangeProfilePictureUseCase(ILoggedUser loggedUser)
    {
        _loggedUser = loggedUser;
    }

    public Task Execute(Stream profilePicture)
    {
        if (profilePicture.DetectImageContentType() == null)
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);

        throw new NotImplementedException();
    }
}
