namespace Lidas.WishlistApi.Entities;

public class Wish
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MangaId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Wish(Guid userId, Guid mangaId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        MangaId = mangaId;

        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(Guid userId, Guid mangaId)
    {
        UserId = userId;
        MangaId = mangaId;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
