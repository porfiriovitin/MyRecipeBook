using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.User;

public class RegisterUserUseCase
{
    public PayloadResponse<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {

        /// :: Validate the request.
        Validate(request);

        /// :: Maps the request to the domain model.


        /// :: Hash the password.


        /// :: Save the user to the database.


        return new PayloadResponse<ResponseRegisteredUserJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = "Usuário cadastrado com sucesso",
            Data = new ResponseRegisteredUserJson(request.Name, request.Email)
        };
    }

    private static void Validate(RequestRegisterUserJson request)
    {
        /// :: Validate the request using FluentValidation.
        var validator = new RegisterUserValidator();
        var result = validator.Validate(request);

        /// :: If the validation fails, throw an exception with the error messages.
        if (!result.IsValid)
        {
            throw new ArgumentException(string.Join(" | ", result.Errors.Select(e => e.ErrorMessage)));
        }

    }
}
