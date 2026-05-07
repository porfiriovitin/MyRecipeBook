using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases;

public class RegisterUserUseCase
{
    public PayloadResponse<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {

        /// :: Validate the request.


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
}
