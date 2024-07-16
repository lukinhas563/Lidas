using FluentValidation;
using Lidas.MangaApi.Models.InputModels;

namespace Lidas.MangaApi.Validators;

public class MangaValidator: AbstractValidator<MangaInput>
{
    public MangaValidator()
    {
        RuleFor(manga => manga.Banner)
            .NotEmpty().WithMessage("Banner is required.");

        RuleFor(manga => manga.Cover)
            .NotEmpty().WithMessage("Cover is required.");

        RuleFor(manga => manga.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(manga => manga.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(manga => manga.Release)
            .NotEmpty().WithMessage("Release is required.");

    }
}
