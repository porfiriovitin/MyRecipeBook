using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using CommomTestsUtilities.Security;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Tests.ChangePassword;

public class ChangePasswordUseCaseTests
{
    [Fact]
    public async Task Sucess()
    {
        var (user, password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.CurrentPassword = password;

        var useCase = CreateUseCase(user, password);

        await useCase.Execute(request);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenNewPasswordIsEmpty()
    {
        var (user, password) = UserBuilder.Build();

        var request = new MyRecipeBook.Communication.Requests.RequestChangePasswordJson
        {
            CurrentPassword = password,
            NewPassword = string.Empty
        };

        var useCase = CreateUseCase(user, password);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.ShouldSatisfyAllConditions(error =>
        {
            error.GetStatusCode().ShouldBe(System.Net.HttpStatusCode.BadRequest);
            error.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.ShouldContain(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED);
            });
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenCurrentPasswordIsIncorrect()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.ShouldSatisfyAllConditions(error =>
        {
            error.GetStatusCode().ShouldBe(System.Net.HttpStatusCode.BadRequest);
            error.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.ShouldContain(ResourceMessagesException.VALIDATION_CURRENT_PASSWORD);
            });
        });
    }

    private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? currentPassword = null)
    {
        var userUpdateOnlyRepository = IUserUpdateOnlyRepositoryBuilder.Build();
        var loggedUser = ILoggedUserBuilder.Build(user);
        var passwordHasherBuilder = new IPasswordHasherBuilder();

        if (currentPassword is not null)
            passwordHasherBuilder.VerifyPassword(currentPassword);

        var passwordHasher = passwordHasherBuilder.Build();

        return new ChangePasswordUseCase(loggedUser, passwordHasher, userUpdateOnlyRepository);
    }
}
