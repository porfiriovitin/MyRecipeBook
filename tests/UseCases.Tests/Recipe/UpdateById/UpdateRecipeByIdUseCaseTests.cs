using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyRecipeBook.Application;
using MyRecipeBook.Application.UseCases.Recipe.UpdatebyId;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using DomainRecipe = MyRecipeBook.Domain.Entities.Recipe;
using DomainUser = MyRecipeBook.Domain.Entities.User;

namespace UseCases.Tests.Recipe.UpdateById;

public class UpdateRecipeByIdUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        new ServiceCollection().AddApplication();

        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user.Id);

        var recipeId = recipe.Id;

        var request = RequestRecipeJsonBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();

        var useCase = CreateUseCase(recipe, user, unitOfWork);

        await useCase.Execute(recipe.Id, request);

        recipe.Id.ShouldBe(recipeId);
        recipe.UserId.ShouldBe(user.Id);
        recipe.Title.ShouldBe(request.Title);
        recipe.CookTime.ShouldBe((MyRecipeBook.Domain.Enums.CookTime)request.CookTime);
        recipe.Ingredients.Select(item => item.Item).ShouldBe(request.Ingredients.Select(item => item.Item));
        recipe.Instructions.OrderBy(item => item.Order).Select(item => (item.Order, item.Description)) .ShouldBe(request.Instructions.OrderBy(item => item.Order).Select(item => (item.Order, item.Description)));
        recipe.DishTypes.Select(item => item.Type).ShouldBe(request.DishTypes.Select(item => (MyRecipeBook.Domain.Enums.DishType)item));
        
        Mock.Get(unitOfWork).Verify(work => work.Commit(), Times.Once);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenTitleIsEmpty()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user.Id);

        var originalTitle = recipe.Title;

        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var repository = new IRecipeUpdateOnlyRepositoryBuilder().GetById(recipe).Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var useCase = new UpdateRecipebyIdUseCase(ILoggedUserBuilder.Build(user), repository, unitOfWork);

        var exception = await useCase.Execute(recipe.Id, request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().Count.ShouldBe(1);
        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_TITLE_REQUIRED);
        recipe.Title.ShouldBe(originalTitle);

        Mock.Get(repository).Verify(item => item.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()), Times.Never);
        Mock.Get(unitOfWork).Verify(work => work.Commit(), Times.Never);
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenRecipeIsNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var useCase = CreateUseCase(null, user, unitOfWork);

        var exception = await useCase.Execute(Guid.NewGuid(), RequestRecipeJsonBuilder.Build()).ShouldThrowAsync<NotFoundException>();

        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
        Mock.Get(unitOfWork).Verify(work => work.Commit(), Times.Never);
    }

    [Fact]
    public async Task ShouldThrowNotFoundException_WhenRecipeBelongsToAnotherUser()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build();

        var originalTitle = recipe.Title;
        var unitOfWork = IUnitOfWorkBuilder.Build();

        var useCase = CreateUseCase(recipe, user, unitOfWork);

        var exception = await useCase.Execute(recipe.Id, RequestRecipeJsonBuilder.Build()).ShouldThrowAsync<NotFoundException>();

        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
        recipe.Title.ShouldBe(originalTitle);
        Mock.Get(unitOfWork).Verify(work => work.Commit(), Times.Never);
    }

    private static UpdateRecipebyIdUseCase CreateUseCase(DomainRecipe? recipe, DomainUser user, IUnitOfWork unitOfWork)
    {
        var repositoryBuilder = new IRecipeUpdateOnlyRepositoryBuilder();

        if (recipe is not null)
            repositoryBuilder.GetById(recipe);

        return new UpdateRecipebyIdUseCase(ILoggedUserBuilder.Build(user), repositoryBuilder.Build(), unitOfWork);
    }
}
