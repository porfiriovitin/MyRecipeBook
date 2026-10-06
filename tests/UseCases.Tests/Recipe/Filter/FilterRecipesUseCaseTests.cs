using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using Moq;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Dtos;
using Shouldly;

namespace UseCases.Tests.Recipe.Filter;

public class FilterRecipesUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = Enumerable.Range(1, 3).Select(_ => RecipeBuilder.Build(user.Id)).ToList();
        var request = RequestFilterRecipesJsonBuilder.Build();
        var repository = new IRecipeReadOnlyRepositoryBuilder().FilterRecipes(user.Id, recipes).Build();
        var useCase = new FilterRecipesUseCase(ILoggedUserBuilder.Build(user), repository);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Recipes.Select(recipe => (recipe.Id, recipe.Title)).ShouldBe(recipes.Select(recipe => (recipe.Id, recipe.Title)));

        Mock.Get(repository).Verify(item => item.FilterRecipes(user.Id, It.Is<RecipeFilterDto>(filter =>
            filter.SearchTerm == request.SearchTerm &&
            filter.CookTime == (MyRecipeBook.Domain.Enums.CookTime?)request.CookTime &&
            filter.DishTypes.SequenceEqual(request.DishTypes.Select(dishType => (MyRecipeBook.Domain.Enums.DishType)dishType)))), Times.Once);

        Mock.Get(repository).VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ShouldUseEmptyFilter_WhenRequestIsNullOrEmpty(bool nullRequest)
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user.Id);
        var repository = new IRecipeReadOnlyRepositoryBuilder().FilterRecipes(user.Id, [recipe]).Build();
        var useCase = new FilterRecipesUseCase(ILoggedUserBuilder.Build(user), repository);

        var result = await useCase.Execute(nullRequest ? null : new RequestFilterRecipesJson());

        result.Recipes.ShouldHaveSingleItem();
        result.Recipes[0].Id.ShouldBe(recipe.Id);
        result.Recipes[0].Title.ShouldBe(recipe.Title);
        Mock.Get(repository).Verify(item => item.FilterRecipes(user.Id, It.Is<RecipeFilterDto>(filter =>
            filter.SearchTerm == null && filter.CookTime == null && !filter.DishTypes.Any())), Times.Once);
        Mock.Get(repository).VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(CookTime.UpTo30Minutes)]
    [InlineData(CookTime.From30To60Minutes)]
    [InlineData(CookTime.MoreThan60Minutes)]
    public async Task ShouldConvertCookTime_ToDomainEnum(CookTime cookTime)
    {
        var (user, _) = UserBuilder.Build();
        var request = new RequestFilterRecipesJson { CookTime = cookTime };
        var repository = new IRecipeReadOnlyRepositoryBuilder().FilterRecipes(user.Id, []).Build();
        var useCase = new FilterRecipesUseCase(ILoggedUserBuilder.Build(user), repository);

        await useCase.Execute(request);

        Mock.Get(repository).Verify(item => item.FilterRecipes(user.Id, It.Is<RecipeFilterDto>(filter =>
            filter.CookTime == (MyRecipeBook.Domain.Enums.CookTime)cookTime &&
            filter.SearchTerm == null && !filter.DishTypes.Any())), Times.Once);
        Mock.Get(repository).VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldConvertDishTypes_ToDomainEnums()
    {
        var (user, _) = UserBuilder.Build();
        var request = new RequestFilterRecipesJson { DishTypes = Enum.GetValues<DishType>().ToList() };
        var repository = new IRecipeReadOnlyRepositoryBuilder().FilterRecipes(user.Id, []).Build();
        var useCase = new FilterRecipesUseCase(ILoggedUserBuilder.Build(user), repository);

        await useCase.Execute(request);

        Mock.Get(repository).Verify(item => item.FilterRecipes(user.Id, It.Is<RecipeFilterDto>(filter =>
            filter.SearchTerm == null && filter.CookTime == null &&
            filter.DishTypes.SequenceEqual(Enum.GetValues<MyRecipeBook.Domain.Enums.DishType>()))), Times.Once);
        Mock.Get(repository).VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoRecipesMatch()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestFilterRecipesJsonBuilder.Build();
        var repository = new IRecipeReadOnlyRepositoryBuilder().FilterRecipes(user.Id, []).Build();
        var useCase = new FilterRecipesUseCase(ILoggedUserBuilder.Build(user), repository);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeNull();
        result.Recipes.ShouldBeEmpty();
        Mock.Get(repository).Verify(item => item.FilterRecipes(user.Id, It.IsAny<RecipeFilterDto>()), Times.Once);
        Mock.Get(repository).VerifyNoOtherCalls();
    }
}
