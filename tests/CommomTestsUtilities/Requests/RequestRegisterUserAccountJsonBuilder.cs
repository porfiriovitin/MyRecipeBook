using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommomTestsUtilities.Requests;

public class RequestRegisterUserAccountJsonBuilder
{
    public static RequestRegisterUserAccountJson Build()
    {
        var faker = new Faker();

        return new RequestRegisterUserAccountJson(
            Name: faker.Person.FirstName,
            Email: faker.Internet.Email(),
            Password: faker.Internet.Password(10, prefix: "Aa1!")
        );
    }
}
