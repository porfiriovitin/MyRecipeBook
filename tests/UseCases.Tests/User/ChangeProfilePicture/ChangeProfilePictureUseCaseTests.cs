using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Files;
using MyRecipeBook.Application.UseCases.User.ChangeProfilePicture;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Tests.User.ChangeProfilePicture;

public class ChangeProfilePictureUseCaseTests
{
    [Theory]
    [InlineData("txt")]
    [InlineData("gif")]
    public void Validate_ShouldThrowException_WhenFileIsNotJpegOrPng(string format)
    {
        var (user, _) = UserBuilder.Build();
        var useCase = new ChangeProfilePictureUseCase(ILoggedUserBuilder.Build(user));
        using var image = ImageStreamBuilder.Build(format);

        var exception = Should.Throw<ErrorOnValidationException>(() => useCase.Execute(image));

        exception.GetErrorMessages().Count.ShouldBe(1);
        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED);
    }

    [Theory]
    [InlineData("jpeg")]
    [InlineData("png")]
    public void ShouldReachUnimplementedUpload_WhenImageIsValid(string format)
    {
        var (user, _) = UserBuilder.Build();
        var useCase = new ChangeProfilePictureUseCase(ILoggedUserBuilder.Build(user));
        using var image = ImageStreamBuilder.Build(format);

        Should.Throw<NotImplementedException>(() => useCase.Execute(image));
    }
}
