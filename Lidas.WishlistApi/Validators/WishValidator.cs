using FluentValidation;
using Lidas.WishlistApi.Interfaces;
using Lidas.WishlistApi.Models.Input;

namespace Lidas.WishlistApi.Validators;

public class WishValidator: AbstractValidator<WishInput>
{
    public WishValidator()
    {
        RuleFor(wish => wish.UserId).NotEmpty();
        RuleFor(wish => wish.MangaId).NotEmpty();
    }
}
