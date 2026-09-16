using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class RecipeController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRegisteredRecipeJson>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RequestRecipeJson request, [FromServices] IRegisterRecipeUseCase registerRecipeUseCase)
    {
        ResponseRegisteredRecipeJson result = await registerRecipeUseCase.Execute(request);

        return StatusCode(StatusCodes.Status201Created, new PayloadResponse<ResponseRegisteredRecipeJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = "Recipe registered successfully.",
            Data = result
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRecipeJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return Ok();
    }
}
