using FluentValidation;
using Lidas.UserApi.Models.Input;

namespace Lidas.UserApi.Validators;

public class LoginValidator: AbstractValidator<LoginInput>
{
    public LoginValidator()
    {
        RuleFor(login => login.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Username cannot exceed 200 characters.");

        RuleFor(login => login.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
