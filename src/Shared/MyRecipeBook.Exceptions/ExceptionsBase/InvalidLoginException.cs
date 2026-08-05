using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class InvalidLoginException : MyRecipeBookException
{
    public InvalidLoginException()
    {
    }

    public override List<string> GetErrorMessages() => [ResourceMessagesException.VALIDATION_LOGIN_INVALID];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}
