using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTestsUtilities.Requests;

public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build()
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(x => x.CurrentPassword, f => f.Internet.Password())
            .RuleFor(x => x.NewPassword, f => f.Internet.Password());
    }

}
