using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Models.InputModels;

public class MangaInput
{
    public IFormFile Banner { get; set; }
    public IFormFile Cover { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Release { get; set; }
}
