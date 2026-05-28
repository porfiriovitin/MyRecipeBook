using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User;

public class RegisterUserAccountUseCase
{
    public ResponseRegisteredUserJson Execute(RequestRegisterUserAccountJson request)
    {
        /// :: Validate the request.
        ValidateAndThrowOnFailures(request);

        /// :: Maps the request to the domain model.
        var user = request.Adapt<Domain.Entities.User>();

        /// :: Hash the passwords


        /// :: Save the user to the database.


        return new ResponseRegisteredUserJson(
            Name: request.Name,
            Email: request.Email
        );
      
    }

    private static void ValidateAndThrowOnFailures(RequestRegisterUserAccountJson request)
    {
        /// :: Validate the request using FluentValidation.
        var validator = new RegisterUserAccountValidator();
        var result = validator.Validate(request);

        /// :: If the validation fails, throw an exception with the error messages.
        if (!result.IsValid)
        {
            throw new ErrorOnValidationException(string.Join(" | ", result.Errors.Select(e => e.ErrorMessage)));
        }

    }
}
