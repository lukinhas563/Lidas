using FluentValidation;
using Lidas.MangaApi.Models.InputModels;

namespace Lidas.MangaApi.Validators;

public class ChapterValidator: AbstractValidator<ChapterInput>
{
    public ChapterValidator()
    {
        RuleFor(chapter => chapter.Number)
            .NotEmpty().WithMessage("Number is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Number must be at least 0.");

        RuleFor(chapter => chapter.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
    }
}
