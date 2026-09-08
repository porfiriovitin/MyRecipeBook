using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Tests.Update;

public class UpdateUserUseCaseTests
{

    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        await useCase.Execute(request);

        user.Name.ShouldBe(request.Name);
        user.Email.ShouldBe(request.Email);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenNameIsEmpty()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.ShouldSatisfyAllConditions(error =>
        {
            exception.GetStatusCode().ShouldBe(System.Net.HttpStatusCode.BadRequest);
            exception.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.ShouldContain(string.Format(ResourceMessagesException.VALIDATION_NAME_REQUIRED));
            });
        });
    
        user.Name.ShouldNotBe(request.Name);
        user.Email.ShouldNotBe(request.Email);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenEmailAlreadyExists()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user, emailThatAlreadyExists: request.Email);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.ShouldSatisfyAllConditions(error =>
        {
            exception.GetStatusCode().ShouldBe(System.Net.HttpStatusCode.BadRequest);
            exception.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.ShouldContain(ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS);
            });
        });

        user.Name.ShouldNotBe(request.Name);
        user.Email.ShouldNotBe(request.Email);
    }

    private static UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? emailThatAlreadyExists = null)
    {
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var userUpdateRepositoy = IUserUpdateOnlyRepositoryBuilder.Build();
        var loggedUser = ILoggedUserBuilder.Build(user);

        var userReadOnlyRepositoryBuilder = new IUserReadOnlyRepositoryBuilder();

        if (emailThatAlreadyExists.IsNotEmpty())
            userReadOnlyRepositoryBuilder.ExistActiveUserWithEmail(emailThatAlreadyExists);

        return new UpdateUserUseCase(loggedUser: loggedUser, userReadOnlyRepository: userReadOnlyRepositoryBuilder.Build(), userUpdateOnlyRepository: userUpdateRepositoy, unitOfWork: unitOfWork);

    }

}
