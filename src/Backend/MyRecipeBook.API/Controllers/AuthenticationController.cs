using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;
using MyRecipeBook.Application.UseCases.PasswordRecovery.RequestCode;
using MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost("login")]
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

    [HttpPost("password-recovery/reset")]
    [EnableRateLimiting("publicEndpoints")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(PayloadResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromServices] IResetPasswordUseCase useCase, [FromBody] RequestResetPasswordJson request)
    {
        await useCase.Execute(request);

        return StatusCode(StatusCodes.Status204NoContent);
    }



}

