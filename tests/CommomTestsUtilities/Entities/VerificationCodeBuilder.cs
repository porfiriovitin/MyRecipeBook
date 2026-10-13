using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;

namespace CommomTestsUtilities.Entities;

public class VerificationCodeBuilder
{
    public static VerificationCode Build(Guid userId)
    {
        return new Faker<VerificationCode>()
            .RuleFor(code => code.UserId, _ => userId)
            .RuleFor(code => code.Type, _ => VerificationCodeType.PasswordRecovery)
            .RuleFor(code => code.Code, faker => faker.Random.Int(1, 999999).ToString("D6"))
            .RuleFor(code => code.CreatedAt, _ => DateTime.UtcNow);
    }
}
