using CommomTestsUtilities.IPasswordHasherBuilder;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User;

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

        /// Assert


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
