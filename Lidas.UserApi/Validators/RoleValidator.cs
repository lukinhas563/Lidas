using FluentValidation;
using Lidas.UserApi.Models.Input;

namespace Lidas.UserApi.Validators;

public class RoleValidator: AbstractValidator<RoleInput>
{
    public RoleValidator()
    {
        RuleFor(role => role.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");
    }
}
