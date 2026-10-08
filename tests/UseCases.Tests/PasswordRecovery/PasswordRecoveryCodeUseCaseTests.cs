using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using Moq;
using MyRecipeBook.Application.UseCases.PasswordRecovery;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;
using Shouldly;

namespace UseCases.Tests.PasswordRecovery;

public class PasswordRecoveryCodeUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestPasswordRecoveryJsonBuilder.Build();
        request.Email = user.Email;

        var userRepositoryBuilder = new IUserReadOnlyRepositoryBuilder();
        userRepositoryBuilder.GetByEmail(user);
        var userRepository = userRepositoryBuilder.Build();

        VerificationCode? savedCode = null;
        var verificationCodeRepositoryBuilder = new IVerificationCodeWriteOnlyRepositoryBuilder();
        verificationCodeRepositoryBuilder.Add(code => savedCode = code);
        var verificationCodeRepository = verificationCodeRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();

        var useCase = new PasswordRecoveryCodeUseCase(verificationCodeRepository, unitOfWork, userRepository);

        await useCase.Execute(request);

        savedCode.ShouldNotBeNull();
        savedCode.UserId.ShouldBe(user.Id);
        savedCode.Type.ShouldBe(VerificationCodeType.PasswordRecovery);
        savedCode.Code.ShouldMatch("^[0-9]{6}$");
        int.Parse(savedCode.Code).ShouldBeInRange(1, 999999);

        Mock.Get(userRepository).Verify(repository => repository.GetByEmail(request.Email), Times.Once);
        Mock.Get(verificationCodeRepository).Verify(repository => repository.Add(It.IsAny<VerificationCode>()), Times.Once);
        Mock.Get(unitOfWork).Verify(repository => repository.Commit(), Times.Once);
    }

    [Fact]
    public async Task Success_WhenUserDoesNotExist()
    {
        var request = RequestPasswordRecoveryJsonBuilder.Build();
        var userRepository = new IUserReadOnlyRepositoryBuilder().Build();
        var verificationCodeRepository = new IVerificationCodeWriteOnlyRepositoryBuilder().Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();

        var useCase = new PasswordRecoveryCodeUseCase(verificationCodeRepository, unitOfWork, userRepository);

        await useCase.Execute(request);

        Mock.Get(userRepository).Verify(repository => repository.GetByEmail(request.Email), Times.Once);
        Mock.Get(verificationCodeRepository).Verify(repository => repository.Add(It.IsAny<VerificationCode>()), Times.Never);
        Mock.Get(unitOfWork).Verify(repository => repository.Commit(), Times.Never);
    }
}
