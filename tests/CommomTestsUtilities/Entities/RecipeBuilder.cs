using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;

namespace CommomTestsUtilities.Entities;

public class RecipeBuilder
{
    public static Recipe Build(Guid? userId = null)
    {
        var recipe = new Faker<Recipe>()
            .RuleFor(recipe => recipe.Title, faker => faker.Lorem.Sentence())
            .RuleFor(recipe => recipe.CookTime, faker => faker.PickRandom<CookTime>())
            .RuleFor(recipe => recipe.UserId, _ => userId ?? Guid.NewGuid())
            .Generate();

        var faker = new Faker();

        recipe.Ingredients = Enumerable.Range(1, 3)
            .Select(_ => new RecipeIngredient
            {
                Item = faker.Commerce.ProductName()
            })
            .ToList();

        recipe.Instructions = Enumerable.Range(1, 3)
            .Select(order => new RecipeInstruction
            {
                Order = order,
                Description = faker.Lorem.Sentence()
            })
            .ToList();

        recipe.DishTypes = Enum.GetValues<DishType>()
            .OrderBy(_ => faker.Random.Int())
            .Take(2)
            .Select(dishType => new RecipeDishType
            {
                Type = dishType,
                RecipeId = recipe.Id
            })
            .ToList();

        return recipe;
    }
}
