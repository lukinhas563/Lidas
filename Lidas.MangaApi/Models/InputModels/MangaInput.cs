using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Models.InputModels;

public class MangaInput
{
    public string Banner { get; set; }
    public string Cover { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Release { get; set; }
}
