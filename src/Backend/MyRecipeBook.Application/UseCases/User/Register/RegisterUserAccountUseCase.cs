using FluentValidation.Results;
using Mapster;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User;

public class RegisterUserAccountUseCase : IRegisterUserAccountUseCase
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserAccountUseCase(IPasswordHasher passwordHasher, IUserWriteOnlyRepository userWriteOnlyRepository, IUserReadOnlyRepository userReadOnlyRepository, IUnitOfWork unitOfWork)
    {
        _passwordHasher = passwordHasher;
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserAccountJson request)
    {
        /// :: Validate the request.
        await ValidateAndThrowOnFailures(request);

        /// :: Maps the request to the domain model.
        var user = request.Adapt<Domain.Entities.User>();

        /// :: Hash the passwords
        user.Password = _passwordHasher.HashPassword(request.Password);

        /// :: Save the user to the database.
        await _userWriteOnlyRepository.Add(user);

        /// :: Commit the transaction.
        await _unitOfWork.Commit();

        return new ResponseRegisteredUserJson(
            Name: request.Name,
            Tokens: new ResponseTokensJson(AccessToken: "", RefreshToken: "")
        );
      
    }

    private async Task ValidateAndThrowOnFailures(RequestRegisterUserAccountJson request)
    {
        /// :: Validate the request using FluentValidation.
        var validator = new RegisterUserAccountValidator();
        var result = validator.Validate(request);

        /// :: Check if the email already exists in the database.
        bool emailAlreadyExists = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
        if (emailAlreadyExists) { 
            result.Errors.Add(new ValidationFailure("Email", ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS));
        }

        /// :: If the validation fails, throw an exception with the error messages.
        if (!result.IsValid )
        {
            throw new ErrorOnValidationException(string.Join(" | ", result.Errors.Select(e => e.ErrorMessage)));
        }


    }

 
}
