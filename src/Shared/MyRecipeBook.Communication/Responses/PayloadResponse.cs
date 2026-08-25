namespace MyRecipeBook.Communication.Responses;

public class PayloadResponse
{
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}

public class PayloadResponse<T> : PayloadResponse
{
    public T? Data { get; init; }
}
