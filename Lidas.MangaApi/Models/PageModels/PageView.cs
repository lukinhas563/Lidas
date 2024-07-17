namespace Lidas.MangaApi.Models.PageModels;

public class PageView<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public List<T> Data { get; set; }

    public PageView(int pageNumber, int pageSize, int count, List<T> data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int) Math.Ceiling(count / (double) pageSize);
        TotalCount = count;
        Data = data;
    }
}
