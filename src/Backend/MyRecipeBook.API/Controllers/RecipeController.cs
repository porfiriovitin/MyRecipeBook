using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Communication.Enums;
using Microsoft.AspNetCore.Authorization;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Application.UseCases.Recipe.Recent;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Application.UseCases.Recipe.UpdatebyId;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class RecipeController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRegisteredRecipeJson>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromForm] RequestRecipeJson request, [FromServices] IRegisterRecipeUseCase useCase, IFormFile? recipeIlustration)
    {
        ResponseRegisteredRecipeJson result = await useCase.Execute(request, recipeIlustration?.OpenReadStream());

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
    public async Task<IActionResult> GetById([FromRoute(Name = "id")] Guid recipeId, [FromServices] IGetRecipeByIdUseCase useCase)
    {
        ResponseRecipeJson result = await useCase.Execute(recipeId);

        return StatusCode(StatusCodes.Status200OK, new PayloadResponse<ResponseRecipeJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = "Recipe found successfully.",
            Data = result
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute(Name = "id")] Guid recipeId, [FromServices] IDeleteRecipebyIdUseCase useCase)
    {
        await useCase.Execute(recipeId);

        return StatusCode(StatusCodes.Status204NoContent);
    }


    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PayloadResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute(Name = "id")] Guid recipeId, [FromBody]RequestRecipeJson request, [FromServices] IUpdateRecipeByIdUseCase useCase)
    {
        await useCase.Execute(recipeId, request);

        return StatusCode(StatusCodes.Status204NoContent);
    }

    [HttpGet("recent")]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRecipesJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent([FromServices] IGetRecentRecipesUseCase useCase)
    {
        var recipes = await useCase.Execute();

        return StatusCode(StatusCodes.Status200OK, new PayloadResponse<ResponseRecipesJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = "Recent recipes found successfully.",
            Data = recipes
        });
    }

    [HttpPost("filter")]
    [ProducesResponseType(typeof(PayloadResponse<ResponseRecipesJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Filter([FromBody] RequestFilterRecipesJson? request, [FromServices] IFilterRecipesUseCase useCase)
    {
        var recipes = await useCase.Execute(request);

        return StatusCode(StatusCodes.Status200OK, new PayloadResponse<ResponseRecipesJson>
        {
            Status = nameof(ResponseStatus.Success),
            Message = "Filtered recipes found successfully.",
            Data = recipes
        });
    }
}
