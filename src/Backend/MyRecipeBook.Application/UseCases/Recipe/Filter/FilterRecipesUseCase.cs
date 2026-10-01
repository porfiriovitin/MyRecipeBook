using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipesUseCase : IFilterRecipesUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _repository;

    public FilterRecipesUseCase(ILoggedUser loggedUser, IRecipeReadOnlyRepository repository)
    {
        _loggedUser = loggedUser;
        _repository = repository;
    }

    public async Task<ResponseRecipesJson> Execute(RequestFilterRecipesJson? request)
    {
        var recipes = await _repository.FilterRecipes(_loggedUser.GetUserId());

        return new ResponseRecipesJson
        {
            Recipes = recipes.Adapt<IList<ResponseRecipeSummaryJson>>()
        };

    }
}
