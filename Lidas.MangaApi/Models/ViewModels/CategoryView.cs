using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Models.ViewModels;

public class CategoryView
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<MangaViewList> Mangas { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
