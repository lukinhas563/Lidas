namespace Lidas.MangaApi.Entities;

public class Manga
{
    public Guid Id { get; set; }
    public string Banner {  get; set; }
    public string Cover { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Release { get; set; }
    public List<Category> Categories { get; set; }
    public List<Author> Authors { get; set; }
    public List<Chapter> Chapters { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Manga(string banner, string cover, string name, string description, DateTime release)
    {
        Id = Guid.NewGuid();
        Banner = banner;
        Cover = cover;
        Name = name;
        Description = description;
        Release = release;

        Categories = new List<Category>();
        Authors = new List<Author>();
        Chapters = new List<Chapter>();

        IsDeleted = false;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public void Update(string banner, string cover, string name, string description, DateTime release)
    {
        Banner = banner;
        Cover = cover;
        Name = name;
        Description = description;
        Release = release;

        UpdatedAt = DateTime.Now;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.Now;
    }
}
