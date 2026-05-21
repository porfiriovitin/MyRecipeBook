namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException : MyRecipeBookException
{
    public ErrorOnValidationException(string message) : base(message)
    {
    }
}
