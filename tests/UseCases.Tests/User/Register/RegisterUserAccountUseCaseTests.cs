using CommomTestsUtilities.IPasswordHasherBuilder;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User;
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
        result.Tokens.AccessToken.ShouldBeNullOrEmpty();
        result.Tokens.RefreshToken.ShouldBeNullOrEmpty();
    }

    private RegisterUserAccountUseCase CreateUseCase()
    {
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var userWriteOnlyRepository = IUserWriteOnlyRepositoryBuilder.Build();
        var userReadOnlyRepository = new IUserReadOnlyRepositoryBuilder().Build();
        var passwordHasher = new IPasswordHasherBuilder().Build();

        return new RegisterUserAccountUseCase(passwordHasher: passwordHasher, userWriteOnlyRepository: userWriteOnlyRepository, userReadOnlyRepository: userReadOnlyRepository, unitOfWork: unitOfWork);
    }

}
