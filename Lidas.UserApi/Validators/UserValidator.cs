using Lidas.UserApi.Validations;

namespace Lidas.UserApi.Validators;

public class UserValidator
{
    public RegisterValidator Register;
    public LoginValidator Login;

    public UserValidator(RegisterValidator registerValidator, LoginValidator loginValidator)
    {
        Register = registerValidator;
        Login = loginValidator;
    }
}
