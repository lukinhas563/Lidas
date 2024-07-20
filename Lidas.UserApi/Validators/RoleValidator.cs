using FluentValidation;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Persist;
using Microsoft.EntityFrameworkCore;

namespace Lidas.UserApi.Validators;

public class RoleValidator: AbstractValidator<RoleInput>
{
    private readonly AppDbContext _context;
    public RoleValidator(AppDbContext context)
    {
        _context = context;

        RuleFor(role => role.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.")
            .MustAsync(async (name, cancellation) => !await RoleExist(name)).WithMessage("Name already exists.");
    }

    private async Task<bool> RoleExist(string roleName)
    {
        return await _context.Roles.AnyAsync(role => role.Name == roleName);
    }
}
