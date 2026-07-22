using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace CommomTestsUtilities.Repositories;

public class IUserReadOnlyRepositoryBuilder
{
    private readonly Mock<IUserReadOnlyRepository> _userReadOnlyRepositoryMock;

    public IUserReadOnlyRepositoryBuilder()
    {
        _userReadOnlyRepositoryMock = new Mock<IUserReadOnlyRepository>();
    }

    public void ExistActiveUserWithEmail(string email)
    {
        _userReadOnlyRepositoryMock.Setup(repository => repository.ExistActiveUserWithEmail(email)).ReturnsAsync(true);
    }

    public IUserReadOnlyRepository Build() => _userReadOnlyRepositoryMock.Object;
}
