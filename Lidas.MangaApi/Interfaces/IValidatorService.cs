using Lidas.MangaApi.Validators;

namespace Lidas.MangaApi.Interfaces;

public interface IValidatorService
{
    AuthorValidator Author { get; }
    CategoryValidator Category { get;  }
    ChapterValidator Chapter { get; }
    MangaValidator Manga { get; }
    RoleValidator Role { get; }
}
