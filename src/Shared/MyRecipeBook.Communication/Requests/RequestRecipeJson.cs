using MyRecipeBook.Communication.Enums;

namespace MyRecipeBook.Communication.Requests;

public class RequestRecipeJson
{
    public string Title { get; set; } = string.Empty;
    public List<RequestRecipeIngredientJson> Ingredients { get; set; } = [];
    public List<RequestRecipeInstructionJson> Instructions { get; set; } = [];
    public List<DishType> DishTypes { get; set; } = [];
    public CookTime CookTime { get; set; }
}
