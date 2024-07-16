using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Models.InputModels;

public class AuthorInput
{
    public string Name { get; set; }
    public string Biography { get; set; }
    public DateTime Birthday { get; set; }
}
