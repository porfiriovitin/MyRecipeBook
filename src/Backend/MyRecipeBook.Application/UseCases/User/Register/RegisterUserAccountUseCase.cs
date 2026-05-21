using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User;

public class RegisterUserAccountUseCase
{
    public PayloadResponse<ResponseRegisteredUserJson> Execute(RequestRegisterUserAccountJson request)
    {

        /// :: Validate the request.
        Validate(request);

        /// :: Maps the request to the domain model.


        /// :: Hash the password.


        /// :: Save the user to the database.


        return new PayloadResponse<ResponseRegisteredUserJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = ResourceMessagesException.USER_REGISTERED_SUCESSFULLY,
            Data = new ResponseRegisteredUserJson(request.Name, request.Email)
        };
    }

    private static void Validate(RequestRegisterUserAccountJson request)
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
