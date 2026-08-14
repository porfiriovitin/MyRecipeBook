using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infraestructure.DataAcess;

namespace MyRecipeBook.Infraestructure.Identity;

internal sealed class LoggedUser : ILoggedUser
{
    private readonly IAcessTokenProvider _acessTokenProvider;
    private readonly MyRecipeBookDbContext _dbContext;

    public LoggedUser(IAcessTokenProvider acessTokenProvider, MyRecipeBookDbContext dbContext)
    {
        _acessTokenProvider = acessTokenProvider;
        _dbContext = dbContext;
    }

    public async Task<User> Get()
    {
        var userId = GetUserId();

        return await _dbContext.Users.AsNoTracking().FirstAsync(user => user.Active && user.Id == userId);
    }

    public Guid GetUserId()
    {
        var acessToken = _acessTokenProvider.GetToken();

        var handler = new JsonWebTokenHandler();

        var jsonWebToken = handler.ReadJsonWebToken(acessToken);

        var subject = jsonWebToken.Claims.First(c => c.Type == "sub").Value;

        return Guid.Parse(subject);
    }
}
