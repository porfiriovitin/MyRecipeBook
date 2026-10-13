using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<RequestResetPasswordJson>
{
    public ResetPasswordValidator()
    {
        RuleFor(request => request.Code).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_CODE_REQUIRED);
        RuleFor(request => request.Email).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_RESET_PASSWORD_EMAIL_REQUIRED);
        RuleFor(request => request.NewPassword).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_NEW_PASSWORD_REQUIRED);
    }
}
