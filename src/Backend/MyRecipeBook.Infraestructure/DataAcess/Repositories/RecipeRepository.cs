using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infraestructure.DataAcess.Repositories;

internal sealed class RecipeRepository : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository, IRecipeUpdateOnlyRepository
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

    public async Task<bool> DeleteById(Guid recipeId, Guid userId)
    {
        var rowsAffected = await _dbContext.Recipes.Where(recipe => recipe.Id == recipeId && recipe.UserId == userId && recipe.Active).ExecuteDeleteAsync();

        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Recipe>> FilterRecipes(Guid userId, RecipeFilterDto filter)
    {
        var query = _dbContext.Recipes.AsNoTracking().Where(recipe => recipe.Active && recipe.UserId == userId);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(recipe => recipe.Title.Contains(filter.SearchTerm) || recipe.Ingredients.Any(i => i.Item.Contains(filter.SearchTerm)));

        if (filter.CookTime is not null)
            query = query.Where(recipe => recipe.CookTime == filter.CookTime);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetRecentRecipes(Guid userId)
    {
        return await _dbContext.Recipes
           .AsNoTracking()
           .Where(recipe => recipe.Active && recipe.UserId == userId)
           .OrderByDescending(recipe => recipe.CreatedAt)
           .Take(6).ToListAsync();
    }

    async Task<Recipe?> IRecipeReadOnlyRepository.GetByIdAsync(Guid recipeId, Guid? userId)
    {
        return await GetFullRecipe().AsNoTracking().FirstOrDefaultAsync(recipe => recipe.Active && recipe.Id == recipeId && recipe.UserId == userId);
    }

    async Task<Recipe?> IRecipeUpdateOnlyRepository.GetByIdAsync(Guid recipeId, Guid? userId)
    {
        return await GetFullRecipe().FirstOrDefaultAsync(recipe => recipe.Active && recipe.Id == recipeId && recipe.UserId == userId);
    }

    private IIncludableQueryable<Recipe, ICollection<RecipeDishType>> GetFullRecipe()
    {
        return _dbContext.Recipes
           .Include(recipe => recipe.Ingredients)
           .Include(recipe => recipe.Instructions.OrderBy(i => i.Order))
           .Include(recipe => recipe.DishTypes);
    }
}
