using Lidas.MangaApi.Interfaces;

namespace Lidas.MangaApi.Validators;

public class Validator: IValidatorService
{
   public AuthorValidator Author { get; }
    public CategoryValidator Category { get;  }
    public ChapterValidator Chapter { get;  }
    public MangaValidator Manga { get; }
    public RoleValidator Role { get; }

    public Validator
        (
        AuthorValidator author, 
        CategoryValidator category,
        ChapterValidator chapter, 
        MangaValidator manga, 
        RoleValidator role
        )
    {
        Author = author;
        Category = category;
        Chapter = chapter;
        Manga = manga;
        Role = role;
    }
}
