using Lidas.WishlistApi.Interfaces;

namespace Lidas.WishlistApi.Validators;

public class ValidatorService : IValidatorService
{
    public WishValidator Wish {  get; set; }

    public ValidatorService(WishValidator wish)
    {
        Wish = wish;
    }
}
