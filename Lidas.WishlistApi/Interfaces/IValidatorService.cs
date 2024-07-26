using Lidas.WishlistApi.Validators;

namespace Lidas.WishlistApi.Interfaces;

public interface IValidatorService
{
    WishValidator Wish { get; }
}
