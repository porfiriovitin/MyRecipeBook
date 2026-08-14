using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Application.UseCases.User.Register;
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
    public async Task<IActionResult> Register([FromBody] RequestRegisterUserAccountJson request, [FromServices] IRegisterUserAccountUseCase registerUserAccountUseCase)
    {
            ResponseRegisteredUserJson result = await registerUserAccountUseCase.Execute(request);

            return StatusCode(StatusCodes.Status201Created, new PayloadResponse<ResponseRegisteredUserJson>
            {
                Status = nameof(ResponseStatus.Success),
                Message = "User account registered successfully.",
                Data = result
            });
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PayloadResponse<ResponseUserProfileJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserProfile([FromServices] IGetUserProfileUseCase useCase)
    {
        var result = await useCase.Execute();

        return StatusCode(StatusCodes.Status201Created, new PayloadResponse<ResponseUserProfileJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = "User account registered successfully.",
            Data = result
        });
    }
}