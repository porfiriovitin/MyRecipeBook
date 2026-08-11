using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using CommomTestsUtilities.Security;
using MyRecipeBook.Application.UseCases.User;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Tests.User.Register;

public class RegisterUserAccountUseCaseTests
{

    [Fact]
    public async Task Sucess()
    {
        /// Arrange
        var request = RequestRegisterUserAccountJsonBuilder.Build();
        var useCase = CreateUseCase();

        /// Act
        var result = await useCase.Execute(request);

        /// Assert
        result.ShouldNotBeNull();
        result.Tokens.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);

        /// :: NOT IMPLEMENTED YET.
        result.Tokens.AccessToken.ShouldNotBeNull();
        result.Tokens.RefreshToken.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenNameIsEmpty()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build() with { Name = string.Empty };
        var useCase = CreateUseCase();

        var exception =  await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesException.VALIDATION_NAME_REQUIRED);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenEmailAlreadyExists()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();
        var useCase = CreateUseCase(request.Email);

        var exception =  await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS);
        });
    }

    private static RegisterUserAccountUseCase CreateUseCase(string? emailThatAlreadyExists = null)
    {
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var userWriteOnlyRepository = IUserWriteOnlyRepositoryBuilder.Build();
        var passwordHasher = new IPasswordHasherBuilder().Build();
        var userReadOnlyRepositoryBuilder = new IUserReadOnlyRepositoryBuilder();
        var acessTokenGeneratorBuilder = IAcessTokenGeneratorBuilder.Build();

        if (emailThatAlreadyExists.IsNotEmpty())
            userReadOnlyRepositoryBuilder.ExistActiveUserWithEmail(emailThatAlreadyExists);

        return new RegisterUserAccountUseCase(passwordHasher: passwordHasher, userWriteOnlyRepository: userWriteOnlyRepository, userReadOnlyRepository: userReadOnlyRepositoryBuilder.Build(), unitOfWork: unitOfWork, acessTokenGeneratorBuilder);
    }

}
