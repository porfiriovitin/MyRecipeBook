namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException : MyRecipeBookException
{
    private readonly List<string> _errors;

    public ErrorOnValidationException(List<string> errors) : base(string.Join(", ", errors))
    {
        _errors = errors;
    }

    public List<string> GetErrorMessages() => _errors;

}
