using Lidas.UserApi.Validations;
using Lidas.UserApi.Validators;

namespace Lidas.UserApi.Interfaces;

public interface IValidatorService
{
    RegisterValidator Register { get; }
    LoginValidator Login { get; }
    EmailValidator Email { get; }
    PasswordValidator Password { get; }
    RoleValidator Role { get; }
}
