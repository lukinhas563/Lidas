using FluentValidation;
using Lidas.UserApi.Models.Input;

namespace Lidas.UserApi.Validators;

public class PasswordValidator: AbstractValidator<PasswordInput>
{
    public PasswordValidator()
    {
        RuleFor(password => password.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Equal(password => password.ConfirmPassword);

        RuleFor(confirm => confirm.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .MinimumLength(6).WithMessage("Confirm password must be at least 6 characters long.")
            .Equal(confirm => confirm.Password);
    }
}
