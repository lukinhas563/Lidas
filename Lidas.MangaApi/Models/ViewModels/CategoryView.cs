using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.PageModels;

namespace Lidas.MangaApi.Models.ViewModels;

public class CategoryView
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public PageView<MangaViewList> Mangas { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
