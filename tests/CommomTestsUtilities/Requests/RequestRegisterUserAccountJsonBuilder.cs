using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTestsUtilities.Requests;

public class RequestRegisterUserAccountJsonBuilder
{
    public static RequestRegisterUserAccountJson Build(int passwordLength = 10)
    {
        return new Faker<RequestRegisterUserAccountJson>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Password, f => f.Internet.Password(length: passwordLength));
    }
}
