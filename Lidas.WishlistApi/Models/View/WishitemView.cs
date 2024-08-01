namespace Lidas.WishlistApi.Models.View;

public class WishitemView
{
    public Guid Id { get; set; }
    public Guid MangaId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
