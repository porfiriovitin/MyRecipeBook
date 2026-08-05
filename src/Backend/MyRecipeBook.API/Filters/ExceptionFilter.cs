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
        var payload = new PayloadResponse<object>
        {
            Status = nameof(ResponseStatus.Error),
            Message = string.Empty,
            Data = null
        };

        if (context.Exception is MyRecipeBookException myRecipeBookException)
        {
            context.HttpContext.Response.StatusCode = (int)myRecipeBookException.GetStatusCode();
            payload.Message = string.Join( " | ", myRecipeBookException.GetErrorMessages());
            context.Result = new ObjectResult(payload);
        }
        else
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            payload.Message = ResourceMessagesException.UNKNOWN_ERROR;
            context.Result = new ObjectResult(payload);
        }

        context.ExceptionHandled = true;
    }
}
