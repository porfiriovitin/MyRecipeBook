using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommomTestsUtilities.Repositories;

public class IRecipeUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeUpdateOnlyRepository> _mock = new();

    public IRecipeUpdateOnlyRepositoryBuilder GetById(Recipe recipe)
    {
        _mock.Setup(repository => repository.GetByIdAsync(recipe.Id, recipe.UserId)).ReturnsAsync(recipe);

        return this;
    }

    public IRecipeUpdateOnlyRepository Build() => _mock.Object;
}
