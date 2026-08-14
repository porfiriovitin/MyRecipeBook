namespace MyRecipeBook.Communication.Responses;

public class PayloadResponse<T>
{
    public string Status { get; init; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; init; }
}
