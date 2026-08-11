using Bogus;
using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.Tokens;

namespace CommomTestsUtilities.Security;

public static class IAcessTokenGeneratorBuilder
{
    public static IAcessTokenGenerator Build()
    {
        var mock = new Mock<IAcessTokenGenerator>();

        var fakeToken = new Faker().Random.String2(32, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");

        mock.Setup(generator => generator.Generate(It.IsAny<User>())).Returns(fakeToken);

        return mock.Object;
    }

}
