namespace Lidas.MangaApi.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Manga> Mangas { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        Mangas = new List<Manga>();

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
