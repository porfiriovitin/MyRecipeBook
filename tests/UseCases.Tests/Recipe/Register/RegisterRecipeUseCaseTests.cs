using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
using Moq;
using MyRecipeBook.Domain.Cache;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Tests.Recipe.Register;

public class RegisterRecipeUseCaseTests
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();
        var cache = new Mock<IRecipesCache>();
        var useCase = CreateUseCase(user, cache.Object);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Title.ShouldBe(request.Title);
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

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_TITLE_REQUIRED);
        });
        cache.Verify(item => item.RemoveRecent(It.IsAny<Guid>()), Times.Never);
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, IRecipesCache cache)
    {
        var recipeWriteOnlyRepository = IRecipeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var loggedUser = ILoggedUserBuilder.Build(user);

        return new RegisterRecipeUseCase(recipeWriteOnlyRepository, unitOfWork, loggedUser, cache);
    }
}
