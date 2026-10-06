using MyRecipeBook.Domain.Dtos;

namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeReadOnlyRepository
{
    Task<Entities.Recipe?> GetByIdAsync(Guid recipeId, Guid? userId);

    Task<IEnumerable<RecipeSummaryDto>> GetRecentRecipes(Guid userId);

    Task<IEnumerable<RecipeSummaryDto>> FilterRecipes(Guid userId, RecipeFilterDto filter);
}
