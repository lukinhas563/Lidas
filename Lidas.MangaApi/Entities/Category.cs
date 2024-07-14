namespace Lidas.MangaApi.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name;

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
