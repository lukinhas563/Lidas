using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.PageModels;

namespace Lidas.MangaApi.Models.ViewModels;

public class RoleView
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public PageView<AuthorViewList> Authors { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
