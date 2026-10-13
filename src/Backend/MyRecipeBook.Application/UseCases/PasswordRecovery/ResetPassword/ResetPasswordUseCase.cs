using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Repositories.VerificationCode;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;

public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private const int expirationTimeInMinutes = 10;

    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IVerificationCodeWriteOnlyRepository _verificationCodeWriteOnlyRepository;
    private readonly IVerificationCodeReadOnlyRepository _verificationCodeReadOnlyRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordUseCase(IUserUpdateOnlyRepository userUpdateOnlyRepository, IUserReadOnlyRepository userReadOnlyRepository, IVerificationCodeWriteOnlyRepository verificationCodeWriteOnlyRepository, IVerificationCodeReadOnlyRepository verificationCodeReadOnlyRepository, IPasswordHasher passwordHasher)
    {
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _verificationCodeWriteOnlyRepository = verificationCodeWriteOnlyRepository;
        _verificationCodeReadOnlyRepository = verificationCodeReadOnlyRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Execute(RequestResetPasswordJson request)
    {
        var user = await _userReadOnlyRepository.GetByEmail(request.Email);
        if (user == null)
            throw new ErrorOnValidationException([ResourceMessagesException.VERIFICATION_CODE_INVALID]);

        var verificationCode = await _verificationCodeReadOnlyRepository.GetVerificationCodeAsync(user.Id, request.Code, Domain.Enums.VerificationCodeType.PasswordRecovery);
        if (verificationCode == null)
            throw new ErrorOnValidationException([ResourceMessagesException.VERIFICATION_CODE_INVALID]);

        ValidateAndThrowOnFailures(request, verificationCode);

        var hashedPassword = _passwordHasher.HashPassword(request.NewPassword);

        await _userUpdateOnlyRepository.UpdatePassword(user.Id, hashedPassword);

        await _verificationCodeWriteOnlyRepository.Delete(verificationCode);
    }

    private static void ValidateAndThrowOnFailures(RequestResetPasswordJson request, VerificationCode verificationCode)
    {
       var isCodeValid = verificationCode.CreatedAt.AddMinutes(expirationTimeInMinutes) > DateTime.UtcNow;
       if (!isCodeValid)
           throw new ErrorOnValidationException([ResourceMessagesException.VERIFICATION_CODE_EXPIRED]);

       var result = new ResetPasswordValidator().Validate(request);
        if (!result.IsValid)
            throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
    }
}
