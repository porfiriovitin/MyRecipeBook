using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.DeletebyId;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Tests.Recipe.DeleteById;

public class DeleteRecipeByIdUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user.Id);
        var repository = new IRecipeWriteOnlyRepositoryBuilder()
            .DeleteById(recipe.Id, user.Id, true)
            .BuildRepository();
        var useCase = new DeleteRecipeByIdUseCase(repository, ILoggedUserBuilder.Build(user));

        await useCase.Execute(recipe.Id);
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenRecipeIsNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var recipeId = Guid.NewGuid();
        var repository = new IRecipeWriteOnlyRepositoryBuilder()
            .DeleteById(recipeId, user.Id, false)
            .BuildRepository();
        var useCase = new DeleteRecipeByIdUseCase(repository, ILoggedUserBuilder.Build(user));

        var exception = await useCase.Execute(recipeId).ShouldThrowAsync<NotFoundException>();

        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
    }
}
