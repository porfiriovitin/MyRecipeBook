using Mapster;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Cache;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Recent;

public class GetRecentRecipesUseCase : IGetRecentRecipesUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _repository;
    private readonly IRecipesCache _cache;

    public GetRecentRecipesUseCase(ILoggedUser loggedUser, IRecipeReadOnlyRepository repository, IRecipesCache cache)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _cache = cache;
    }

    public async Task<ResponseRecipesJson> Execute()
    {
        var cachedRecipes = _cache.GetRecent(_loggedUser.GetUserId());

        if (cachedRecipes != null)
        {
            return new ResponseRecipesJson
            {
                Recipes = [.. cachedRecipes]
            };
        }

        var recipes =  await _repository.GetRecentRecipes(_loggedUser.GetUserId());

        var recentRecipes = recipes.Adapt<IList<ResponseRecipeSummaryJson>>();

        _cache.SetRecent(_loggedUser.GetUserId(), recentRecipes);

        return new ResponseRecipesJson
        {
            Recipes = recentRecipes
        };

    }
}
