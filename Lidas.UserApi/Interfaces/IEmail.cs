namespace Lidas.UserApi.Interfaces;

public interface IEmail
{
    public void SendEmail(string userName, string userEmail, string subject, string content);
}
