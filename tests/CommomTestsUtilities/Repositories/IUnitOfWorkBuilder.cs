using Moq;
using MyRecipeBook.Domain.Repositories;

namespace CommomTestsUtilities.Repositories;

public class IUnitOfWorkBuilder
{
    public static IUnitOfWork Build()
    {
        var mock = new Mock<IUnitOfWork>();
        return mock.Object;
    }
}
