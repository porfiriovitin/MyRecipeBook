using Moq;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommomTestsUtilities.Repositories;

public class IRecipeReadOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeReadOnlyRepository> _mock;

    public IRecipeReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IRecipeReadOnlyRepository>();
    }

    public IRecipeReadOnlyRepositoryBuilder GetById(Recipe recipe)
    {
        _mock.Setup(repository => repository.GetByIdAsync(recipe.Id, recipe.UserId)).ReturnsAsync(recipe);

        return this;
    }

    public IRecipeReadOnlyRepositoryBuilder GetRecentRecipes(Guid userId, IEnumerable<Recipe> recipes)
    {
        var recipesDto = recipes.Select(r => new RecipeSummaryDto(r.Id, r.Title)).ToList();

        _mock.Setup(repository => repository.GetRecentRecipes(userId)).ReturnsAsync(recipesDto);

        return this;
    }

    public IRecipeReadOnlyRepositoryBuilder FilterRecipes(Guid userId, IEnumerable<Recipe> recipes)
    {
        var recipesDto = recipes.Select(r => new RecipeSummaryDto(r.Id, r.Title)).ToList();

        _mock.Setup(repository => repository.FilterRecipes(userId, It.IsAny<RecipeFilterDto>())).ReturnsAsync(recipesDto);

        return this;
    }

    public IRecipeReadOnlyRepository Build() => _mock.Object;

}
