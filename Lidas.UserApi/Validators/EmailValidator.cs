using FluentValidation;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Persist;
using Microsoft.EntityFrameworkCore;

namespace Lidas.UserApi.Validators;

public class EmailValidator: AbstractValidator<EmailInput>
{
    public EmailValidator()
    {
        RuleFor(email => email.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.")
            .MinimumLength(3).WithMessage("Email must be at least 3 characters long.")
            .MaximumLength(250).WithMessage("Email cannot exceed 250 characters.");
    }

}
