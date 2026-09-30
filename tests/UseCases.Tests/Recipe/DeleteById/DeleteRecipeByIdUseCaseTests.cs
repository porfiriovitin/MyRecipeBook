using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using Moq;
using MyRecipeBook.Domain.Cache;
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
        var cache = new Mock<IRecipesCache>();
        var useCase = new DeleteRecipeByIdUseCase(repository, ILoggedUserBuilder.Build(user), cache.Object);

        await useCase.Execute(recipe.Id);

        cache.Verify(item => item.RemoveRecent(user.Id), Times.Once);
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenRecipeIsNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var recipeId = Guid.NewGuid();
        var repository = new IRecipeWriteOnlyRepositoryBuilder()
            .DeleteById(recipeId, user.Id, false)
            .BuildRepository();
        var cache = new Mock<IRecipesCache>();
        var useCase = new DeleteRecipeByIdUseCase(repository, ILoggedUserBuilder.Build(user), cache.Object);

        var exception = await useCase.Execute(recipeId).ShouldThrowAsync<NotFoundException>();

        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
        cache.Verify(item => item.RemoveRecent(It.IsAny<Guid>()), Times.Never);
    }
}
