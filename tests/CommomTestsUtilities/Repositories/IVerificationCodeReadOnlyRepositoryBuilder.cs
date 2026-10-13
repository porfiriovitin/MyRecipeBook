using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.VerificationCode;

namespace CommomTestsUtilities.Repositories;

public class IVerificationCodeReadOnlyRepositoryBuilder
{
    private readonly Mock<IVerificationCodeReadOnlyRepository> _mock = new();

    public void GetVerificationCodeAsync(VerificationCode code)
    {
        _mock.Setup(repository => repository.GetVerificationCodeAsync(code.UserId, code.Code, code.Type))
            .ReturnsAsync(code);
    }

    public IVerificationCodeReadOnlyRepository Build() => _mock.Object;
}
