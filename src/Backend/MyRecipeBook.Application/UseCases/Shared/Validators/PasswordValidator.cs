using FluentValidation;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Shared.Validators;

public static class PasswordValidator
{
    internal static IRuleBuilderOptions<TRequest, string> Password<TRequest>(this IRuleBuilderInitial<TRequest, string> ruleBuilder)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED)
            .MinimumLength(6).WithMessage(ResourceMessagesException.VALIDATION_PASSWORD_MIN_LENGTH);
    }
}
