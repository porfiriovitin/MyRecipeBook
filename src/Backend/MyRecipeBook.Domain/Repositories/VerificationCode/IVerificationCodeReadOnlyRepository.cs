using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.Repositories.VerificationCode;

public interface IVerificationCodeReadOnlyRepository
{
    Task<Entities.VerificationCode?> GetVerificationCodeAsync(Guid UserId, string Code, VerificationCodeType type);
}
