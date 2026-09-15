using CommomTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Diagnostics.CodeAnalysis;

namespace Validators.Tests.Recipe;

public class RequestRecipeValidatorTests
{
    [Fact]
    public void Success()
    {
        /// :: Arrange.
        var request = RequestRecipeJsonBuilder.Build();
        var validator = new RequestRecipeValidator();

        /// :: Act.
        var result = validator.Validate(request);

        /// :: Assert.
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Intentional because is a unit test")]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty(string title)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = title;
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_TITLE_REQUIRED);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIngredientsAreEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients = [];
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_INGREDIENTS_REQUIRED);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Intentional because is a unit test")]
    public void Validate_ShouldHaveError_WhenIngredientIsEmpty(string item)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients = [new RequestRecipeIngredientJson { Item = item }];
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_INGREDIENT_REQUIRED);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenInstructionsAreEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions = [];
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_INSTRUCTIONS_REQUIRED);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenInstructionOrderIsNotGreaterThanZero(int order)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions = [new RequestRecipeInstructionJson { Order = order, Description = "Instruction" }];
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_INSTRUCTION_ORDER_GREATER_THAN_ZERO);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Intentional because is a unit test")]
    public void Validate_ShouldHaveError_WhenInstructionDescriptionIsEmpty(string description)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions = [new RequestRecipeInstructionJson { Order = 1, Description = description }];
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_INSTRUCTION_REQUIRED);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenInstructionOrderIsDuplicated()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions = [
            new RequestRecipeInstructionJson { Order = 1, Description = "First instruction" },
            new RequestRecipeInstructionJson { Order = 1, Description = "Second instruction" }
        ];
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_INSTRUCTION_ORDER_DUPLICATED);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDishTypesAreEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes = [];
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_DISH_TYPES_REQUIRED);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDishTypeIsInvalid()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes = [(DishType)99];
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_DISH_TYPE_INVALID);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCookTimeIsInvalid()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.CookTime = (CookTime)99;
        var validator = new RequestRecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == ResourceMessagesException.VALIDATION_RECIPE_COOK_TIME_INVALID);
    }
}
