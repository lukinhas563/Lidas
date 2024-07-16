using FluentValidation;
using Lidas.MangaApi.Models.InputModels;

namespace Lidas.MangaApi.Validators
{
    public class AuthorValidator: AbstractValidator<AuthorInput>
    {
        public AuthorValidator()
        {
            RuleFor(author => author.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

            RuleFor(author => author.Biography)
                .NotEmpty().WithMessage("Biography is required.")
                .MinimumLength(10).WithMessage("Biography must be at least 10 characters long.");

            RuleFor(author => author.Birthday)
                .NotEmpty().WithMessage("Birthday is required.")
                .LessThan(new DateTime()).WithMessage("Birth day must be less than now.");
        }
    }
}
