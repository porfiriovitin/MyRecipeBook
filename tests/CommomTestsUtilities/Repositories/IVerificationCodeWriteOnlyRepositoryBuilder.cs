using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.VerificationCode;

namespace CommomTestsUtilities.Repositories;

public class IVerificationCodeWriteOnlyRepositoryBuilder
{
    private readonly Mock<IVerificationCodeWriteOnlyRepository> _mock = new();

    public void Add(Action<VerificationCode> callback)
    {
        _mock.Setup(repository => repository.Add(It.IsAny<VerificationCode>()))
            .Callback(callback)
            .Returns(Task.CompletedTask);
    }

    public IVerificationCodeWriteOnlyRepository Build() => _mock.Object;
}
