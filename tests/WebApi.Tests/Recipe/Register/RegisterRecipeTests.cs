using CommomTestsUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Recipe.Register;

public class RegisterRecipeTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/recipe";
    private readonly UserIdentityManager _user;

    public RegisterRecipeTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user = factory.FirstUser;
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestRecipeJsonBuilder.Build();
        using var form = CreateFormData(request);

        var response = await PostForm(REQUEST_URI, form, token: _user.GetAcessToken());
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("status").GetString().ShouldBe(nameof(ResponseStatus.Success));
        responseData.RootElement.GetProperty("message").GetString().ShouldBe("Recipe registered successfully.");

        var bodyData = responseData.RootElement.GetProperty("data");
        bodyData.GetProperty("title").GetString().ShouldBe(request.Title);

        var recipeId = bodyData.GetProperty("id").GetGuid();
        var recipe = await DbContext.Recipes
            .Include(recipe => recipe.Ingredients)
            .Include(recipe => recipe.Instructions)
            .Include(recipe => recipe.DishTypes)
            .SingleAsync(recipe => recipe.Id == recipeId);

        recipe.Title.ShouldBe(request.Title);
        recipe.UserId.ShouldBe(_user.GetId());
        recipe.CookTime.ShouldBe((MyRecipeBook.Domain.Enums.CookTime)request.CookTime);
        recipe.Ingredients.Select(ingredient => ingredient.Item)
            .ShouldBe(request.Ingredients.Select(ingredient => ingredient.Item));
        recipe.Instructions.OrderBy(instruction => instruction.Order)
            .Select(instruction => (instruction.Order, instruction.Description))
            .ShouldBe(request.Instructions.OrderBy(instruction => instruction.Order)
                .Select(instruction => (instruction.Order, instruction.Description)));
        recipe.DishTypes.Select(dishType => dishType.Type)
            .ShouldBe(request.DishTypes.Select(dishType => (MyRecipeBook.Domain.Enums.DishType)dishType));
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenTitleIsEmpty(string culture)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;
        using var form = CreateFormData(request);

        var response = await PostForm(REQUEST_URI, form, token: _user.GetAcessToken(), culture: culture);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("status").GetInt32().ShouldBe((int)HttpStatusCode.BadRequest);
        responseData.RootElement.GetProperty("errors").GetProperty(nameof(request.Title))
            .EnumerateArray().Select(error => error.GetString())
            .ShouldContain("The Title field is required.");

        var recipeExists = await DbContext.Recipes.AnyAsync(recipe => recipe.UserId == _user.GetId() && recipe.Title.Equals(request.Title));
        recipeExists.ShouldBeFalse();
    }

    private static MultipartFormDataContent CreateFormData(RequestRecipeJson request)
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(request.Title), nameof(request.Title) },
            { new StringContent(request.CookTime.ToString()), nameof(request.CookTime) }
        };

        for (var index = 0; index < request.Ingredients.Count; index++)
            form.Add(new StringContent(request.Ingredients[index].Item), $"Ingredients[{index}].Item");

        for (var index = 0; index < request.Instructions.Count; index++)
        {
            var instruction = request.Instructions[index];
            form.Add(new StringContent(instruction.Order.ToString(CultureInfo.InvariantCulture)), $"Instructions[{index}].Order");
            form.Add(new StringContent(instruction.Description), $"Instructions[{index}].Description");
        }

        for (var index = 0; index < request.DishTypes.Count; index++)
            form.Add(new StringContent(request.DishTypes[index].ToString()), $"DishTypes[{index}]");

        return form;
    }
}
