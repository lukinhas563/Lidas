using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Models.ViewModels;

public class AuthorView
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Biography { get; set; }
    public DateTime Birthday { get; set; }
    public List<RoleViewList> Roles { get; set; }
    public List<MangaViewList> Mangas { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
