using Microsoft.Extensions.Caching.Memory;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Cache;

namespace MyRecipeBook.Infraestructure.Cache
{
    public class RecipesCache : IRecipesCache
    {
        private readonly IMemoryCache _cache;

        public RecipesCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<ResponseRecipeSummaryJson>? GetRecent(Guid userId)
        {
            _cache.TryGetValue(GetRecentKey(userId),out IEnumerable<ResponseRecipeSummaryJson>? recipes);

            return recipes;
        }

        public void RemoveRecent(Guid userId)
        {
            _cache.Remove(GetRecentKey(userId));
        }

        public void SetRecent(Guid userId, IEnumerable<ResponseRecipeSummaryJson> recipes)
        {
            _cache.Set(GetRecentKey(userId), recipes, TimeSpan.FromMinutes(10));
        }

        private static string GetRecentKey(Guid userId)=> $"recipes:recent:{userId}";
    }
}
