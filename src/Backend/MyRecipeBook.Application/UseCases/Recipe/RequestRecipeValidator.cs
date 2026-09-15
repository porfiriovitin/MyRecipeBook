using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe;

public class RequestRecipeValidator : AbstractValidator<RequestRecipeJson>
{
    public RequestRecipeValidator()
    {
        RuleFor(recipe => recipe.Title)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_RECIPE_TITLE_REQUIRED);

        RuleFor(recipe => recipe.Ingredients)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_RECIPE_INGREDIENTS_REQUIRED);

        RuleForEach(recipe => recipe.Ingredients)
            .Cascade(CascadeMode.Stop)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(item => item.Item)
                    .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_RECIPE_INGREDIENT_REQUIRED);
            });

        RuleFor(recipe => recipe.Instructions)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_RECIPE_INSTRUCTIONS_REQUIRED);

        RuleForEach(recipe => recipe.Instructions)
            .Cascade(CascadeMode.Stop)
            .ChildRules(instruction =>
            {
                instruction.RuleFor(item => item.Order)
                    .GreaterThan(0).WithMessage(ResourceMessagesException.VALIDATION_RECIPE_INSTRUCTION_ORDER_GREATER_THAN_ZERO);

                instruction.RuleFor(item => item.Description)
                    .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_RECIPE_INSTRUCTION_REQUIRED);
            });

        RuleFor(recipe => recipe.DishTypes)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(ResourceMessagesException.VALIDATION_RECIPE_DISH_TYPES_REQUIRED)
        .ForEach(dishType => dishType
        .IsInEnum()
        .WithMessage(ResourceMessagesException.VALIDATION_RECIPE_DISH_TYPE_INVALID));

        RuleFor(recipe => recipe.CookTime)
            .IsInEnum().WithMessage(ResourceMessagesException.VALIDATION_RECIPE_COOK_TIME_INVALID);
    }
}
