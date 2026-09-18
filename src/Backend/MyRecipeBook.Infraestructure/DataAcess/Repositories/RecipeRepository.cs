using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infraestructure.DataAcess.Repositories;

internal sealed class RecipeRepository : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository
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

    public async Task<Recipe?> GetByIdAsync(Guid recipeId, Guid? userId)
    {
        return await _dbContext.Recipes
            .AsNoTracking()
            .Where(recipe => recipe.Id == recipeId && recipe.UserId == userId && recipe.Active)
            .Include(recipe => recipe.Ingredients)
            .Include(recipe => recipe.Instructions.OrderBy(i => i.Order))
            .Include(recipe => recipe.DishTypes).FirstOrDefaultAsync();
    }

}
