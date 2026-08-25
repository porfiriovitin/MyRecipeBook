using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.User.Update;

public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_NAME_REQUIRED);
        RuleFor(user => user.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED)
            .EmailAddress().WithMessage(ResourceMessagesException.VALIDATION_EMAIL_INVALID);
    }
}
