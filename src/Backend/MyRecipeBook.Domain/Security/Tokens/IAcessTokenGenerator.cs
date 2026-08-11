using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Domain.Security.Tokens;

public interface IAcessTokenGenerator
{
    string Generate(User user);
}
