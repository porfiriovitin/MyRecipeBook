using Moq;
using MyRecipeBook.Domain.Security.PasswordHashing;

namespace CommomTestsUtilities.IPasswordHasherBuilder;

public class IPasswordHasherBuilder
{
    private readonly Mock<IPasswordHasher> _mock;

    public IPasswordHasherBuilder()
    {
        _mock = new Mock<IPasswordHasher>();

        _mock.Setup(passwordHasher => passwordHasher.HashPassword(It.IsAny<string>())).Returns("hashed-password");
    }

    public void VerifyPassword(string password)
    {
        _mock.Setup(passwordHasher => passwordHasher.VerifyPassword(password, It.IsAny<string>())).Returns(true);
    }

    public IPasswordHasher Build() => _mock.Object;


}
