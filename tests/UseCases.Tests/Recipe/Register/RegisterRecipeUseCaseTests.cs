using CommomTestsUtilities;
using CommomTestsUtilities.Entities;
using CommomTestsUtilities.Repositories;
using CommomTestsUtilities.Requests;
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
        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Title.ShouldBe(request.Title);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenTitleIsEmpty()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;
        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_TITLE_REQUIRED);
        });
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var recipeWriteOnlyRepository = IRecipeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var loggedUser = ILoggedUserBuilder.Build(user);

        return new RegisterRecipeUseCase(recipeWriteOnlyRepository, unitOfWork, loggedUser);
    }
}
