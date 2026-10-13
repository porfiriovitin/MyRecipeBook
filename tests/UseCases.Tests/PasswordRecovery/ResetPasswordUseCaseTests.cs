using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using CommomTestsUtilities.Security;
using Moq;
using MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Repositories.VerificationCode;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using System.Net;

namespace UseCases.Tests.PasswordRecovery;

public class ResetPasswordUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var code = VerificationCodeBuilder.Build(user.Id);
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = code.Code;
        var dependencies = CreateDependencies(user, code);
        var useCase = CreateUseCase(dependencies);

        await useCase.Execute(request);

        Mock.Get(dependencies.UserRead).Verify(repository => repository.GetByEmail(request.Email), Times.Once);
        Mock.Get(dependencies.CodeRead).Verify(repository => repository.GetVerificationCodeAsync(user.Id, request.Code, VerificationCodeType.PasswordRecovery), Times.Once);
        Mock.Get(dependencies.Hasher).Verify(hasher => hasher.HashPassword(request.NewPassword), Times.Once);
        Mock.Get(dependencies.UserUpdate).Verify(repository => repository.UpdatePassword(user.Id, "hashed-password"), Times.Once);
        Mock.Get(dependencies.CodeWrite).Verify(repository => repository.Delete(code), Times.Once);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenUserDoesNotExist()
    {
        var request = RequestResetPasswordJsonBuilder.Build();
        var dependencies = CreateDependencies();
        var useCase = CreateUseCase(dependencies);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        AssertValidationError(exception, ResourceMessagesException.VERIFICATION_CODE_INVALID);
        Mock.Get(dependencies.UserRead).Verify(repository => repository.GetByEmail(request.Email), Times.Once);
        Mock.Get(dependencies.CodeRead).Verify(repository => repository.GetVerificationCodeAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<VerificationCodeType>()), Times.Never);
        AssertNoChanges(dependencies);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenCodeIsInvalid()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        var dependencies = CreateDependencies(user);
        var useCase = CreateUseCase(dependencies);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        AssertValidationError(exception, ResourceMessagesException.VERIFICATION_CODE_INVALID);
        Mock.Get(dependencies.CodeRead).Verify(repository => repository.GetVerificationCodeAsync(user.Id, request.Code, VerificationCodeType.PasswordRecovery), Times.Once);
        AssertNoChanges(dependencies);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(11)]
    public async Task Validate_ShouldThrowException_WhenCodeIsExpired(int ageInMinutes)
    {
        var (user, _) = UserBuilder.Build();
        var code = VerificationCodeBuilder.Build(user.Id);
        code.CreatedAt = DateTime.UtcNow.AddMinutes(-ageInMinutes);
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = code.Code;
        var dependencies = CreateDependencies(user, code);
        var useCase = CreateUseCase(dependencies);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        AssertValidationError(exception, ResourceMessagesException.VERIFICATION_CODE_EXPIRED);
        AssertNoChanges(dependencies);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_ShouldThrowException_WhenNewPasswordIsEmpty(string? newPassword)
    {
        var (user, _) = UserBuilder.Build();
        var code = VerificationCodeBuilder.Build(user.Id);
        var request = RequestResetPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = code.Code;
        request.NewPassword = newPassword!;
        var dependencies = CreateDependencies(user, code);
        var useCase = CreateUseCase(dependencies);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        AssertValidationError(exception, ResourceMessagesException.VALIDATION_NEW_PASSWORD_REQUIRED);
        AssertNoChanges(dependencies);
    }

    private static void AssertValidationError(ErrorOnValidationException exception, string message)
    {
        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(message);
    }

    private static void AssertNoChanges((IUserReadOnlyRepository UserRead, IUserUpdateOnlyRepository UserUpdate,
        IVerificationCodeReadOnlyRepository CodeRead, IVerificationCodeWriteOnlyRepository CodeWrite, IPasswordHasher Hasher) dependencies)
    {
        Mock.Get(dependencies.Hasher).Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never);
        Mock.Get(dependencies.UserUpdate).Verify(repository => repository.UpdatePassword(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        Mock.Get(dependencies.CodeWrite).Verify(repository => repository.Delete(It.IsAny<VerificationCode>()), Times.Never);
        Mock.Get(dependencies.CodeWrite).Verify(repository => repository.Replace(It.IsAny<VerificationCode>()), Times.Never);
    }

    private static (IUserReadOnlyRepository UserRead, IUserUpdateOnlyRepository UserUpdate,
        IVerificationCodeReadOnlyRepository CodeRead, IVerificationCodeWriteOnlyRepository CodeWrite, IPasswordHasher Hasher)
        CreateDependencies(MyRecipeBook.Domain.Entities.User? user = null, VerificationCode? code = null)
    {
        var userReadBuilder = new IUserReadOnlyRepositoryBuilder();
        if (user is not null)
            userReadBuilder.GetByEmail(user);

        var codeReadBuilder = new IVerificationCodeReadOnlyRepositoryBuilder();
        if (code is not null)
            codeReadBuilder.GetVerificationCodeAsync(code);

        return (userReadBuilder.Build(), IUserUpdateOnlyRepositoryBuilder.Build(), codeReadBuilder.Build(),
            new IVerificationCodeWriteOnlyRepositoryBuilder().Build(), new IPasswordHasherBuilder().Build());
    }

    private static ResetPasswordUseCase CreateUseCase((IUserReadOnlyRepository UserRead, IUserUpdateOnlyRepository UserUpdate,
        IVerificationCodeReadOnlyRepository CodeRead, IVerificationCodeWriteOnlyRepository CodeWrite, IPasswordHasher Hasher) dependencies)
    {
        return new ResetPasswordUseCase(dependencies.UserUpdate, dependencies.UserRead,
            dependencies.CodeWrite, dependencies.CodeRead, dependencies.Hasher);
    }
}
