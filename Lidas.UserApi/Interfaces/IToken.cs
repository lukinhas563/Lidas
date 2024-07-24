using Lidas.UserApi.Config;
using Lidas.UserApi.Entities;
using Microsoft.Extensions.Options;

namespace Lidas.UserApi.Interfaces;

public interface IToken
{
    // Generate
    public string GenerateToken(User user);
    public string GenerateEmailToken(User user);
    public string GeneratePasswordToken(User user);

    // Validate
    public Task<User> ValidateEmailToken(string token);
    public Task<User> ValidadePasswordToken(string token);
}
