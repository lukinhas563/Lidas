namespace Lidas.MangaApi.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Biography { get; set; }
    public DateTime Birthday { get; set; }
    public List<Role> Roles { get; set; }
    public List<Manga> Mangas { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Author(string name, string biography, DateTime birthday)
    {
        Id = Guid.NewGuid();
        Name = name;
        Biography = biography;
        Birthday = birthday;

        Roles = new List<Role>();
        Mangas = new List<Manga>();

        IsDeleted = false;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public void Update(string name, string biography, DateTime birthday)
    {
        Name = name;
        Biography = biography;
        Birthday = birthday;

        UpdatedAt = DateTime.Now;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.Now;
    }
}
