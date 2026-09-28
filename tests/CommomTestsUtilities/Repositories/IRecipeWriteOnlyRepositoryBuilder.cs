using Moq;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommomTestsUtilities.Repositories;

public class IRecipeWriteOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeWriteOnlyRepository> _mock = new();

    public IRecipeWriteOnlyRepositoryBuilder DeleteById(Guid recipeId, Guid userId, bool deleted)
    {
        _mock.Setup(repository => repository.DeleteById(recipeId, userId)).ReturnsAsync(deleted);

        return this;
    }

    public IRecipeWriteOnlyRepository BuildRepository() => _mock.Object;

    public static IRecipeWriteOnlyRepository Build()
    {
        var mock = new Mock<IRecipeWriteOnlyRepository>();
        return mock.Object;
    }
}
