namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeWriteOnlyRepository
{
    Task AddAsync(Entities.Recipe recipe);
    Task<bool> DeleteById(Guid recipeId, Guid UserId);
}
