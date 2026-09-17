using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class NotFoundException : MyRecipeBookException
{
    private readonly string _message;

    public NotFoundException(string message)
    {
        _message = message;
    }

    public override List<string> GetErrorMessages() => [_message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
}
