using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException : MyRecipeBookException
{
    private readonly List<string> _errors;

    public ErrorOnValidationException(List<string> errors) 
    {
        _errors = errors;
    }

    public override List<string> GetErrorMessages() => _errors;

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
}
