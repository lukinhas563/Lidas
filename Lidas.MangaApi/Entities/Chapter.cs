namespace Lidas.MangaApi.Entities;

public class Chapter
{
    public Guid Id { get; set; }
    public int Number {  get; set; }
    public string Title { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Chapter(int number, string title)
    {
        Id = Guid.NewGuid();
        Number = number;
        Title = title;

        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(int number, string title)
    {
        Number = number; 
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
