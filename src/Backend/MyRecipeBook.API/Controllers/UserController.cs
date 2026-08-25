using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;
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

        return StatusCode(StatusCodes.Status200OK, new PayloadResponse<ResponseUserProfileJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = "User profile retrieved successfully.",
            Data = result
        });
    }

    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(PayloadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PayloadResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromBody] RequestUpdateUserJson request, [FromServices] IUpdateUserUseCase useCase)
    {
        await useCase.Execute(request);

        return StatusCode(StatusCodes.Status200OK, new PayloadResponse
        {
            Status = nameof(ResponseStatus.Success),
            Message = "User profile updated successfully.",
        });
    }

    [HttpPut("password")]
    [Authorize]
    [ProducesResponseType(typeof(PayloadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PayloadResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePassword([FromBody] RequestChangePasswordJson request, [FromServices] IChangePasswordUseCase useCase)
    {
        await useCase.Execute(request);

        return StatusCode(StatusCodes.Status200OK, new PayloadResponse
        {
            Status = nameof(ResponseStatus.Success),
            Message = "Password updated successfully.",
        });
    }
}