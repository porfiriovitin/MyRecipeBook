using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using DomainRecipe = MyRecipeBook.Domain.Entities.Recipe;
using DomainUser = MyRecipeBook.Domain.Entities.User;

namespace UseCases.Tests.Recipe.GetById;

public class GetRecipeByIdUseCaseTests
{
    [Fact]
    public async Task Sucess()
    {
        ConfigureMapster();

        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user.Id);
        var useCase = CreateUseCase(recipe, user);

        var result = await useCase.Execute(recipe.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(recipe.Id);
        result.Title.ShouldBe(recipe.Title);
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenRecipeIsNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(null, user);

        var exception = await useCase.Execute(Guid.NewGuid()).ShouldThrowAsync<NotFoundException>();

        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
    }

    private static GetRecipeByIdUseCase CreateUseCase(DomainRecipe? recipe, DomainUser user)
    {
        var loggedUser = ILoggedUserBuilder.Build(user);
        var repositoryBuilder = new IRecipeReadOnlyRepositoryBuilder();

        if (recipe is not null)
            repositoryBuilder.GetById(recipe);

        return new GetRecipeByIdUseCase(repositoryBuilder.Build(), loggedUser);
    }

    private static void ConfigureMapster()
    {
        var services = new ServiceCollection();
        services.AddApplication();
    }
}
