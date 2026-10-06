using Bogus;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;

namespace CommomTestsUtilities.Requests;

public class RequestFilterRecipesJsonBuilder
{
    public static RequestFilterRecipesJson Build()
    {
        return new Faker<RequestFilterRecipesJson>()
            .RuleFor(request => request.SearchTerm, faker => faker.Commerce.ProductName())
            .RuleFor(request => request.CookTime, CookTime.UpTo30Minutes)
            .RuleFor(request => request.DishTypes, [DishType.Breakfast, DishType.Snack]);
    }
}
