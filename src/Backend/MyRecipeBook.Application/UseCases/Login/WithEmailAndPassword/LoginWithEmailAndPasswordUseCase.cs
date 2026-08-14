using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCase : ILoginWithEmailAndPasswordUseCase
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IAcessTokenGenerator _acessTokenGenerator;

    public LoginWithEmailAndPasswordUseCase(IPasswordHasher passwordHasher, IUserReadOnlyRepository userReadOnlyRepository, IAcessTokenGenerator acessTokenGenerator    )
    {
        _passwordHasher = passwordHasher;
        _userReadOnlyRepository = userReadOnlyRepository;
        _acessTokenGenerator = acessTokenGenerator;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        var user = await _userReadOnlyRepository.GetByEmail(request.Email);
        if (user == null) 
            throw new InvalidLoginException();

        var isPasswordValid =  _passwordHasher.VerifyPassword(request.Password, user.Password);
        if (!isPasswordValid)
            throw new InvalidLoginException();

        string acessToken = _acessTokenGenerator.Generate(user);

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
            Tokens = new ResponseTokensJson { AccessToken = acessToken, RefreshToken = "" }
        };
    }
}
