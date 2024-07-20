using Lidas.UserApi.Config;
using Lidas.UserApi.Entities;
using Lidas.UserApi.Persist;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lidas.UserApi.Services;

public class TokenService
{
    private readonly TokenSettings _settings;
    private readonly AppDbContext _context;
    public TokenService(IOptions<TokenSettings> options, AppDbContext context)
    {
        _settings = options.Value;
        _context = context;
    }

    public string GenerateToken(User user)
    {

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Key);
        var signingCredential = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(8),
            SigningCredentials = signingCredential,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateEmailToken(User user)
    {

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.EmailKey);
        var signingCredential = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim("Id", user.Id.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(8),
            SigningCredentials = signingCredential,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<User> ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.EmailKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var userId = principal.FindFirstValue("Id");

            var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == Guid.Parse(userId));

            return user;
        }
        catch
        {
            return null;
        }
    }

}
