namespace Lidas.MangaApi.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Author> Author { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Role(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        Author = new List<Author>();

        IsDeleted = false;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public void Update(string name)
    {
        Name = name;

        UpdatedAt = DateTime.Now;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.Now;
    }
}
