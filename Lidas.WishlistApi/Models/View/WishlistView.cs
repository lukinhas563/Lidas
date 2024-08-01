using Lidas.WishlistApi.Entities;

namespace Lidas.WishlistApi.Models.View;

public class WishlistView
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<WishitemView> Wishitems { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
