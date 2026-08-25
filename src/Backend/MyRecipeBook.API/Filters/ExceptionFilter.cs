using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        int statusCode;
        string message;

        if (context.Exception is MyRecipeBookException myRecipeBookException)
        {
            statusCode = (int)myRecipeBookException.GetStatusCode();
            message = string.Join(" | ", myRecipeBookException.GetErrorMessages());
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = ResourceMessagesException.UNKNOWN_ERROR;
        }

        var payload = new PayloadResponse<object>
        {
            Status = nameof(ResponseStatus.Error),
            Message = message,
            Data = null
        };

        context.Result = new ObjectResult(payload)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
