using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Persist
{
    public class AppDbContext
    {
        public List<Manga> Mangas { get; set; }
        public List<Author> Authors { get; set; }

        public AppDbContext()
        {
            Mangas = new List<Manga>();
            Authors = new List<Author>();
        }
    }
}
