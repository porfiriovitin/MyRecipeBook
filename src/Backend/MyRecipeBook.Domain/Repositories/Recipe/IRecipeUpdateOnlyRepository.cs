namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeUpdateOnlyRepository
{
    Task<Entities.Recipe?> GetByIdAsync(Guid recipeId, Guid? userId);
}
