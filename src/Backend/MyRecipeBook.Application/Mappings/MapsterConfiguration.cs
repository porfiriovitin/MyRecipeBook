using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Application.Mappings;

internal static class MapsterConfiguration
{
    internal static void Configure()
    {
        TypeAdapterConfig<RequestRegisterUserAccountJson, User>.NewConfig().Ignore(dest => dest.Password);

        TypeAdapterConfig<RequestRecipeJson, Recipe>
            .NewConfig()
            .Map(dest => dest.Ingredients, src => src.Ingredients.Select(ingredient => new RecipeIngredient
            {
                Item = ingredient.Item
            }))
            .Map(dest => dest.Instructions, src => src.Instructions.Select(instruction => new RecipeInstruction
            {
                Order = instruction.Order,
                Description = instruction.Description
            }))
            .Map(dest => dest.DishTypes, src => src.DishTypes.Select(dishType => new RecipeDishType
            {
                Type = (Domain.Enums.DishType)dishType
            }));

        TypeAdapterConfig<Recipe, ResponseRecipeJson>
            .NewConfig()
            .Map(dest => dest.Ingredients, src => src.Ingredients.Select(ingredient => ingredient.Item))
            .Map(dest => dest.DishTypes, src => src.DishTypes.Select(dishType => dishType.Type));
    }
}
