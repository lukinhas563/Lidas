using Lidas.UserApi.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Lidas.UserApi.Services;

public class CryptographyService: ICryptography
{
    private const int SALTSIZE = 128 / 8;
    private const int KEYSIZE = 256 / 8;
    private const int ITERATIONS = 10000;
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
    private static char Delimiter = ';';

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SALTSIZE);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, ITERATIONS, _hashAlgorithmName, KEYSIZE);

        return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public bool Verify(string passwordHash,string inputPassword)
    {
        var elements = passwordHash.Split(Delimiter);

        var salt = Convert.FromBase64String(elements[0]);
        var hash = Convert.FromBase64String(elements[1]);

        var hashInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, ITERATIONS, _hashAlgorithmName, KEYSIZE);

        return CryptographicOperations.FixedTimeEquals(hash, hashInput);
    }
}
