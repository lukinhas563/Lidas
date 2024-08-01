using FluentValidation;
using Lidas.WishlistApi.Interfaces;
using Lidas.WishlistApi.Models.Input;

namespace Lidas.WishlistApi.Validators;

public class WishValidator: AbstractValidator<WishitemInput>
{
    public WishValidator()
    {
        RuleFor(wish => wish.MangaId).NotEmpty();
    }
}
