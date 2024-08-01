namespace Lidas.WishlistApi.Entities;

public class Wishlist
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<WishItem> Wishitems { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Wishlist(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Wishitems = new List<WishItem>();

        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(Guid userId)
    {
        UserId = userId;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
