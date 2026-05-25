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
            Message = context.Exception.Message,
            Data = null
        };

        switch(context.Exception)
        {
            case ErrorOnValidationException:
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new BadRequestObjectResult(payload);
                break;
            default:
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = new ObjectResult(new PayloadResponse<object>
                {
                    Status = nameof(ResponseStatus.Error),
                    Message = ResourceMessagesException.UNKNOWN_ERROR,
                    Data = null
                });
                break;
        }

        context.ExceptionHandled = true;
    }
}
