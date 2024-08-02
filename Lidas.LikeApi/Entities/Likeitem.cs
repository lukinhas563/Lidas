namespace Lidas.LikeApi.Entities;

public class Likeitem
{
    public Guid Id { get; set; }
    public Guid MangaId { get; set; }
    public List<Likelist> Likelists { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Likeitem(Guid mangaId)
    {
        Id = Guid.NewGuid();
        MangaId = mangaId;
        Likelists = new List<Likelist>();

        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(Guid mangaId)
    {
        MangaId = mangaId;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;

        UpdatedAt = DateTime.UtcNow;
    }
}
