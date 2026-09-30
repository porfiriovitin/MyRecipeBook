using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using Moq;
using MyRecipeBook.Application.UseCases.Recipe.Recent;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Cache;
using Shouldly;

namespace UseCases.Tests.Recipe.Recent;

public class GetRecentRecipesUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = Enumerable.Range(1, 3).Select(_ => RecipeBuilder.Build(user.Id)).ToList();
        var repository = new IRecipeReadOnlyRepositoryBuilder().GetRecentRecipes(user.Id, recipes).Build();
        var cache = new Mock<IRecipesCache>();
        cache.Setup(item => item.GetRecent(user.Id)).Returns((IEnumerable<ResponseRecipeSummaryJson>?)null);
        var useCase = new GetRecentRecipesUseCase(ILoggedUserBuilder.Build(user), repository, cache.Object);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Recipes.Select(recipe => (recipe.Id, recipe.Title))
            .ShouldBe(recipes.Select(recipe => (recipe.Id, recipe.Title)));
        Mock.Get(repository).Verify(item => item.GetRecentRecipes(user.Id), Times.Once);
        Mock.Get(repository).VerifyNoOtherCalls();
        cache.Verify(item => item.GetRecent(user.Id), Times.Once);
        cache.Verify(item => item.SetRecent(user.Id, It.Is<IEnumerable<ResponseRecipeSummaryJson>>(cached =>
            cached.Select(recipe => new { recipe.Id, recipe.Title }).SequenceEqual(
                recipes.Select(recipe => new { recipe.Id, recipe.Title })))), Times.Once);
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenUserHasNoRecipes()
    {
        var (user, _) = UserBuilder.Build();
        var repository = new IRecipeReadOnlyRepositoryBuilder().GetRecentRecipes(user.Id, []).Build();
        var cache = new Mock<IRecipesCache>();
        cache.Setup(item => item.GetRecent(user.Id)).Returns((IEnumerable<ResponseRecipeSummaryJson>?)null);
        var useCase = new GetRecentRecipesUseCase(ILoggedUserBuilder.Build(user), repository, cache.Object);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeNull();
        result.Recipes.ShouldBeEmpty();
        Mock.Get(repository).Verify(item => item.GetRecentRecipes(user.Id), Times.Once);
        cache.Verify(item => item.SetRecent(user.Id, It.Is<IEnumerable<ResponseRecipeSummaryJson>>(cached => !cached.Any())), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnCachedRecipes_WithoutQueryingRepository()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = Enumerable.Range(1, 3)
            .Select(_ => RecipeBuilder.Build(user.Id))
            .Select(recipe => new ResponseRecipeSummaryJson { Id = recipe.Id, Title = recipe.Title })
            .ToList();
        var cache = new Mock<IRecipesCache>();
        cache.Setup(item => item.GetRecent(user.Id)).Returns(recipes);
        var repository = new IRecipeReadOnlyRepositoryBuilder().Build();
        var useCase = new GetRecentRecipesUseCase(ILoggedUserBuilder.Build(user), repository, cache.Object);

        var result = await useCase.Execute();

        result.Recipes.Select(recipe => (recipe.Id, recipe.Title))
            .ShouldBe(recipes.Select(recipe => (recipe.Id, recipe.Title)));
        Mock.Get(repository).VerifyNoOtherCalls();
        cache.Verify(item => item.GetRecent(user.Id), Times.Once);
        cache.VerifyNoOtherCalls();
    }
}
