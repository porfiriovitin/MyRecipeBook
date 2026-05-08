using FluentValidation;
using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.User;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserAccountJson>
{
    public RegisterUserValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(user => user.Email).NotEmpty().WithMessage("O email é obrigatório.").EmailAddress().WithMessage("O email deve ser válido.");
        RuleFor(user => user.Password).NotEmpty().WithMessage("A senha é obrigatória.").MinimumLength(6).WithMessage("A senha deve conter no mínimo 6 caracteres.");
    }
}
