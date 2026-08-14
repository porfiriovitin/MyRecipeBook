using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.API.Token;

internal sealed class HttpContextTokenProvider : IAcessTokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetToken()
    {
        var acessToken = _httpContextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

        return acessToken["Bearer ".Length..].Trim();
    }
}
