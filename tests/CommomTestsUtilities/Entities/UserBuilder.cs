using Bogus;
using CommomTestsUtilities.Security;
using MyRecipeBook.Domain.Entities;

namespace CommomTestsUtilities.Entities;

public class UserBuilder
{
    public static (User user, string password) Build()
    {
        var (rawPassword, hashedPassword) = GenerateRandomPassword();

        var mockUser = new Faker<User>()
            .RuleFor(user => user.Name, faker => faker.Person.FirstName)
            .RuleFor(user => user.Email, (faker, user) => faker.Internet.Email(user.Name))
            .RuleFor(user => user.Password, _ => hashedPassword)
            .Generate();

        return (mockUser, rawPassword);
    }

    private static (string rawPassword, string hashedPassword) GenerateRandomPassword()
    {
        var passwordEncripter = new IPasswordHasherBuilder().Build();

        var password = new Faker().Internet.Password();

        return (password, passwordEncripter.HashPassword(password));
    }
}