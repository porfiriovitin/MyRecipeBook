using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Repositories.VerificationCode;
using System.Security.Cryptography;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery;

public class PasswordRecoveryCodeUseCase : IPasswordRecoveryCodeUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IVerificationCodeWriteOnlyRepository _verificationCodeRepository;

    public PasswordRecoveryCodeUseCase(IVerificationCodeWriteOnlyRepository repository, IUnitOfWork unitOfWork, IUserReadOnlyRepository userReadOnlyRepository)
    {
        _verificationCodeRepository = repository;
        _unitOfWork = unitOfWork;
        _userReadOnlyRepository = userReadOnlyRepository;
    }

    public async Task Execute(RequestPasswordRecoveryJson request)
    {
        var user = await _userReadOnlyRepository.GetByEmail(request.Email);
        if (user is null)
            return;

        var code = RandomNumberGenerator.GetInt32(1, 1000000);

        var verificationCode = new VerificationCode
        {
            Code = code.ToString("D6"),
            Type = VerificationCodeType.PasswordRecovery,
            UserId = user.Id
        };

        /// :: Email with the code for the user
        
        await _verificationCodeRepository.Add(verificationCode);

        await _unitOfWork.Commit();
    }
}
