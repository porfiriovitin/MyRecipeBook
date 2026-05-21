using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.User;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRegisteredUserJson>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status400BadRequest)]
    public IActionResult Register([FromBody] RequestRegisterUserAccountJson request)
    {
        try
        {
            var useCase = new RegisterUserAccountUseCase();

            PayloadResponse<ResponseRegisteredUserJson> result = useCase.Execute(request);

            if (!result.Status.Equals(nameof(ResponseStatus.Success)))
            {
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }

            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (MyRecipeBookException ex)
        {
            return BadRequest(new PayloadResponse<object>
            {
                Status = nameof(ResponseStatus.Error),
                Message = ex.Message,
                Data = null
            });
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