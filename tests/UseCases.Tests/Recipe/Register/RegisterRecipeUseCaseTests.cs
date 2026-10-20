using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Files;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Cache;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Tests.Recipe.Register;

public class RegisterRecipeUseCaseTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("jpeg")]
    [InlineData("png")]
    public async Task Success_WithOptionalImage(string? format)
    {
        new ServiceCollection().AddApplication();
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();
        var cache = new Mock<IRecipesCache>();
        var repository = IRecipeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        IRegisterRecipeUseCase useCase = CreateUseCase(user, cache.Object, repository, unitOfWork);
        using var image = format is null ? null : ImageStreamBuilder.Build(format);
        var committed = false;
        Mock.Get(unitOfWork).Setup(work => work.Commit())
            .Callback(() => committed = true).Returns(Task.CompletedTask);
        cache.Setup(item => item.RemoveRecent(user.Id)).Callback(() => committed.ShouldBeTrue());

        var result = await useCase.Execute(request, image);

        result.ShouldNotBeNull();
        result.Title.ShouldBe(request.Title);
        Mock.Get(repository).Verify(item => item.AddAsync(It.Is<MyRecipeBook.Domain.Entities.Recipe>(recipe =>
            recipe.Id == result.Id && recipe.Title == request.Title && recipe.UserId == user.Id)), Times.Once);
        Mock.Get(unitOfWork).Verify(work => work.Commit(), Times.Once);
        cache.Verify(item => item.RemoveRecent(user.Id), Times.Once);
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenTitleIsEmpty()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;
        var cache = new Mock<IRecipesCache>();
        var useCase = CreateUseCase(user, cache.Object);

        var exception = await useCase.Execute(request, null).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_TITLE_REQUIRED);
        });
        cache.Verify(item => item.RemoveRecent(It.IsAny<Guid>()), Times.Never);
    }

    [Theory]
    [InlineData("txt")]
    [InlineData("gif")]
    public async Task Validate_ShouldThrowException_WhenFileIsNotJpegOrPng(string format)
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();
        var repository = IRecipeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var cache = new Mock<IRecipesCache>();
        var useCase = CreateUseCase(user, cache.Object, repository, unitOfWork);
        using var image = ImageStreamBuilder.Build(format);

        var exception = await useCase.Execute(request, image).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().Count.ShouldBe(1);
        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.VALIDATION_ONLY_IMAGES_ACCEPTED);
        Mock.Get(repository).VerifyNoOtherCalls();
        Mock.Get(unitOfWork).Verify(work => work.Commit(), Times.Never);
        cache.VerifyNoOtherCalls();
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, IRecipesCache cache,
        IRecipeWriteOnlyRepository? repository = null, IUnitOfWork? unitOfWork = null)
    {
        var loggedUser = ILoggedUserBuilder.Build(user);

        return new RegisterRecipeUseCase(repository ?? IRecipeWriteOnlyRepositoryBuilder.Build(),
            unitOfWork ?? IUnitOfWorkBuilder.Build(), loggedUser, cache);
    }
}
