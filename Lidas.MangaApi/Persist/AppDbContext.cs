using Lidas.MangaApi.Entities;

namespace Lidas.MangaApi.Persist
{
    public class AppDbContext
    {
        public List<Manga> Mangas { get; set; }
        public List<Author> Authors { get; set; }
        public List<Category> Categories { get; set; }
        public List<Chapter> Chapters { get; set; }
        public List<Role> Roles { get; set; }

        public AppDbContext()
        {
            Mangas = new List<Manga>();
            Authors = new List<Author>();
            Categories = new List<Category>();
            Chapters = new List<Chapter>();
            Roles = new List<Role>();
        }
    }
}
