namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeReadOnlyRepository
{
    Task<Entities.Recipe?> GetByIdAsync(Guid recipeId, Guid userId);
}
