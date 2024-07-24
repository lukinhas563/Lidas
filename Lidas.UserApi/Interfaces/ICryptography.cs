namespace Lidas.UserApi.Interfaces;

public interface ICryptography
{
    public string Hash(string passowrd);
    public bool Verify(string passwordHash, string inputPassword);
}
