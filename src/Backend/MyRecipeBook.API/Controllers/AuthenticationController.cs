using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRegisteredUserJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromServices] ILoginWithEmailAndPasswordUseCase useCase, [FromBody] RequestLoginJson request )
    {
        var response = await useCase.Execute(request);

        return StatusCode(StatusCodes.Status200OK, new PayloadResponse<ResponseRegisteredUserJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = "Login sucessfully",
            Data = response
        });
    }
}

