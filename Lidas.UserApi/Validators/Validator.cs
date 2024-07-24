using Lidas.UserApi.Interfaces;
using Lidas.UserApi.Validations;

namespace Lidas.UserApi.Validators;

public class Validator: IValidatorService
{
    public LoginValidator Login { get; }
    public RegisterValidator Register { get; }
    public EmailValidator Email { get; }
    public PasswordValidator Password { get; }
    public RoleValidator Role { get; }

    public Validator
        (
        RegisterValidator register,
        LoginValidator login, 
        EmailValidator email,
        PasswordValidator password,
        RoleValidator role
        )
    {
        Register = register;
        Login = login;
        Email = email;
        Password = password;
        Role = role;
    }
}
