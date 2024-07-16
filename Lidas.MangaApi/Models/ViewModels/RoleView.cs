using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Models.ViewModels;

public class RoleView
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<AuthorViewList> Authors { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
