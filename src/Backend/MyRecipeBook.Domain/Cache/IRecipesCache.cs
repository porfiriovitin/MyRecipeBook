using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Domain.Cache;

public interface IRecipesCache
{
    IEnumerable<ResponseRecipeSummaryJson>? GetRecent(Guid userId);
    void SetRecent(Guid userId, IEnumerable<ResponseRecipeSummaryJson> recipes);
    void RemoveRecent(Guid userId);
}
