using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTestsUtilities.Requests;

public class RequestPasswordRecoveryJsonBuilder
{
    public static RequestPasswordRecoveryJson Build()
    {
        return new Faker<RequestPasswordRecoveryJson>()
            .RuleFor(request => request.Email, faker => faker.Internet.Email());
    }
}
