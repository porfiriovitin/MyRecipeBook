using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRegisteredUserJson>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status400BadRequest)]
    public IActionResult Register([FromBody] RequestRegisterUserJson request)
    {
        try
        {
            var useCase = new RegisterUserUseCase();

            PayloadResponse<ResponseRegisteredUserJson> result = useCase.Execute(request);

            if (!result.Status.Equals(nameof(ResponseStatus.Success)))
            {
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }

            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new PayloadResponse<object>
                {
                    Status = nameof(ResponseStatus.Error),
                    Message = ex.Message,
                    Data = null
                });
        }
    }
}