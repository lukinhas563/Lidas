using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Models.ViewModels;

public class CategoryViewList
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
