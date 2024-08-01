namespace Lidas.WishlistApi.Models.View;

public class MangaViewList
{
    public Guid Id { get; set; }
    public string Banner { get; set; }
    public string Cover { get; set; }
    public string Name { get; set; }
    public List<CategoryView> Categories { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
