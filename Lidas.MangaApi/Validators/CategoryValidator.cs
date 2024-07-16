using FluentValidation;
using Lidas.MangaApi.Models.InputModels;

namespace Lidas.MangaApi.Validators
{
    public class CategoryValidator: AbstractValidator<CategoryInput>
    {
        public CategoryValidator()
        {
            RuleFor(category => category.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters."); ;
        }
    }
}
