using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infraestructure.DataAcess.Repositories;

internal sealed class RecipeRepository : IRecipeWriteOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;

    public RecipeRepository(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Recipe recipe)
    {
        await _dbContext.Recipes.AddAsync(recipe);
    }
}
