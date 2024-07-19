namespace Lidas.UserApi.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<User> Users { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
     
    public Role(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        Users = new List<User>();

        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name)
    {
        Name = name;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;

        UpdatedAt = DateTime.UtcNow;
    }
}
