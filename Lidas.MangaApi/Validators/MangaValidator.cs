using FluentValidation;
using Lidas.MangaApi.Models.InputModels;

namespace Lidas.MangaApi.Validators;

public class MangaValidator: AbstractValidator<MangaInput>
{
    private readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };
    private readonly string[] permittedMimeTypes = { "image/jpeg", "image/png" };

    public MangaValidator()
    {
        RuleFor(manga => manga.Banner)
            .NotEmpty().WithMessage("Banner is required.")
            .Must(IsValidImage).WithMessage("Banner must be a valid image file.");

        RuleFor(manga => manga.Cover)
            .NotEmpty().WithMessage("Cover is required.")
            .Must(IsValidImage).WithMessage("Cover must be a valid image file."); ;

        RuleFor(manga => manga.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(manga => manga.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(manga => manga.Release)
            .NotEmpty().WithMessage("Release is required.");

    }

    private bool IsValidImage(IFormFile file)
    {
        if (file == null) return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var mimeType = file.ContentType.ToLowerInvariant();

        return permittedExtensions.Contains(extension) && permittedMimeTypes.Contains(mimeType);
    }
}
