using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTestsUtilities.Requests;

public class RequestResetPasswordJsonBuilder
{
    public static RequestResetPasswordJson Build()
    {
        return new Faker<RequestResetPasswordJson>()
            .RuleFor(request => request.Email, faker => faker.Internet.Email())
            .RuleFor(request => request.Code, faker => faker.Random.Int(1, 999999).ToString("D6"))
            .RuleFor(request => request.NewPassword, faker => faker.Internet.Password(12));
    }
}
