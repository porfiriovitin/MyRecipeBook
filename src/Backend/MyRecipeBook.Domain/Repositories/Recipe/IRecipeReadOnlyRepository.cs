namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeReadOnlyRepository
{
    Task<Entities.Recipe?> GetByIdAsync(Guid recipeId, Guid? userId);

    Task<IEnumerable<Entities.Recipe>> GetRecentRecipes(Guid userId);

    Task<IEnumerable<Entities.Recipe>> FilterRecipes(Guid userId);
}
