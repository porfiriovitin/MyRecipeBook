using Bogus;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;

namespace CommomTestsUtilities.Requests;

public class RequestRecipeJsonBuilder
{
    public static RequestRecipeJson Build()
    {
        return new Faker<RequestRecipeJson>()
            .RuleFor(x => x.Title, f => f.Lorem.Sentence())
            .RuleFor(x => x.Ingredients, f => [new RequestRecipeIngredientJson { Item = f.Commerce.ProductName() }])
            .RuleFor(x => x.Instructions, f => [new RequestRecipeInstructionJson { Order = 1, Description = f.Lorem.Sentence() }])
            .RuleFor(x => x.DishTypes, [DishType.Breakfast])
            .RuleFor(x => x.CookTime, CookTime.UpTo30Minutes);
    }
}
