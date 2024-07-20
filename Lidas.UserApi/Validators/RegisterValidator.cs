using FluentValidation;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Persist;
using Microsoft.EntityFrameworkCore;

namespace Lidas.UserApi.Validations;

public class RegisterValidator: AbstractValidator<UserInput>
{
    private readonly AppDbContext _context;
    public RegisterValidator(AppDbContext context)
    {
        _context = context;

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(user => user.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(3).WithMessage("Last name must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Last name cannot exceed 200 characters.");

        RuleFor(user => user.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Username cannot exceed 200 characters.")
            .MustAsync(async (username, cancellation) => !await UsernameExist(username)).WithMessage("Username already exists.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.")
            .MinimumLength(3).WithMessage("Email must be at least 3 characters long.")
            .MaximumLength(250).WithMessage("Email cannot exceed 250 characters.")
            .MustAsync(async (email, cancellation) => !await EmailExist(email)).WithMessage("Email already exists.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Equal(user => user.ConfirmPassowrd).WithMessage("Confirm password must be the same.");

        RuleFor(user => user.ConfirmPassowrd)
            .NotEmpty().WithMessage("Confirm password is required.")
            .MinimumLength(6).WithMessage("Confirm password must be at least 6 characters long.")
            .Equal(user => user.Password).WithMessage("Confirm password must be the same.");
    }

    private async Task<bool> UsernameExist(string username)
    {
        return await _context.Users.AnyAsync(user => user.UserName == username);
    }
    private async Task<bool> EmailExist(string email)
    {
        return await _context.Users.AnyAsync(user =>  user.Email == email);
    }
}
