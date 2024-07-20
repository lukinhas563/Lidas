using Lidas.UserApi.Validations;

namespace Lidas.UserApi.Validators;

public class UserValidator
{
    public RegisterValidator Register;
    public LoginValidator Login;
    public EmailValidator Email;
    public PasswordValidator Password;

    public UserValidator(
        RegisterValidator registerValidator,
        LoginValidator loginValidator,
        EmailValidator emailValidator,
        PasswordValidator passwordValidator)
    {
        Register = registerValidator;
        Login = loginValidator;
        Email = emailValidator; 
        Password = passwordValidator;
    }
}
