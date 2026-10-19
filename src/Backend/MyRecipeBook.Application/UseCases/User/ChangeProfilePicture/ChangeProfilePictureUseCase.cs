using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using System.Reflection;

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
        bool isImage = profilePicture.Is<JointPhotographicExpertsGroup>() || profilePicture.Is<PortableNetworkGraphic>();

        if (!isImage)
            throw new ErrorOnValidationException([ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED]);


        throw new NotImplementedException();
    }
}
