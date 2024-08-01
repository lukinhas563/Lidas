namespace Lidas.WishlistApi.Models.View;

public class PageView
{
	public int PageNumber { get; set; }
	public int PageSize { get; set; }
	public int TotalPages { get; set; }
	public int TotalCount { get; set; }
	public List<MangaViewList> Data { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }

}
