using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/manga/{mangaId}/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(Guid mangaId)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => !manga.IsDeleted && manga.Id == mangaId);

            if(manga == null) return NotFound();

            var authors = manga.Authors;

            return Ok(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid mangaId, Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => !manga.IsDeleted && manga.Id == mangaId);

            if (manga == null) return NotFound();

            var author = manga.Authors.Where(author => author.Id == id && !author.IsDeleted);

            if (author == null) return NotFound();

            return Ok(author);
        }

        [HttpPost]
        public IActionResult Create(Guid mangaId, Author author)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            manga.Authors.Add(author);

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid mangaId, Guid id, Author input)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId);

            if (manga == null) return NotFound();

            var author = manga.Authors.SingleOrDefault(author => author.Id == id);

            if (author == null) return NotFound();

            author.Update(input.Name, input.Biography, input.Birthday);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid mangaId, Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId);

            if (manga == null) return NotFound();

            var author = manga.Authors.SingleOrDefault(author => author.Id == id && !author.IsDeleted);

            if (author == null) return NotFound();

            author.Delete();

            return NoContent();
        }
    }
}
