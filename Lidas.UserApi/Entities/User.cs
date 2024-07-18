namespace Lidas.UserApi.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Role> Role { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User(string name, string lastName, string userName, string email, string password)
    {
        Id = Guid.NewGuid();
        Name = name;
        LastName = lastName;
        UserName = userName;
        Email = email;
        Password = password;
        Role = new List<Role>();

        IsDeleted = false;
        IsEmailConfirmed = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string lastName, string userName, string email, string password)
    {
        Name = name;
        LastName = lastName;
        UserName = userName;
        Email = email;
        Password = password;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

}
