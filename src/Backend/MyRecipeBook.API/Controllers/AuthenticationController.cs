using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;
using MyRecipeBook.Application.UseCases.PasswordRecovery;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("publicEndpoints")]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRegisteredUserJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PayloadResponse), StatusCodes.Status401Unauthorized)]
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

    [HttpPost("password-recovery")]
    [EnableRateLimiting("publicEndpoints")]
    [ProducesResponseType(typeof(PayloadResponse), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PasswordRecovery([FromServices] IPasswordRecoveryCodeUseCase useCase, [FromBody] RequestPasswordRecoveryJson request)
    {
        await useCase.Execute(request);

        return StatusCode(StatusCodes.Status202Accepted);
    }

}

