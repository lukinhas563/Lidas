namespace Lidas.MangaApi.Models.ViewModels;

public class ChapterView
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
