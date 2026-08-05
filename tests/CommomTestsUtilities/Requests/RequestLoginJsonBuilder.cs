using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTestsUtilities.Requests;

public class RequestLoginJsonBuilder
{
    public static RequestLoginJson Build()
    {
        return new Faker<RequestLoginJson>()
            .RuleFor(request => request.Email, request => request.Internet.Email())
            .RuleFor(request => request.Password, request => request.Internet.Password());
    }
}
