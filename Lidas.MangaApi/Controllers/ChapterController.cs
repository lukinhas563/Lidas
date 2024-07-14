using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/manga/{mangaId}/chapter")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChapterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(Guid mangaId)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var chapters = manga.Chapters;

            return Ok(chapters);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid mangaId, Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var chapter = manga.Chapters.SingleOrDefault(chapter => chapter.Id == id && !chapter.IsDeleted);

            if (chapter == null) return NotFound();

            return Ok(chapter);
        }

        [HttpPost]
        public IActionResult Create(Guid mangaId, Chapter chapter)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            manga.Chapters.Add(chapter);

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid mangaId, Guid id, Chapter input)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId);

            if (manga == null) return NotFound();

            var chapter = manga.Chapters.SingleOrDefault(chapter => chapter.Id == id && !chapter.IsDeleted);

            if (chapter == null) return NotFound();

            chapter.Update(input.Number, input.Title);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid mangaId, Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId);

            if (manga == null) return NotFound();

            var chapter = manga.Chapters.SingleOrDefault(chapter => chapter.Id == id && !chapter.IsDeleted);

            if (chapter == null) return NotFound();

            chapter.Delete();

            return NoContent();
        }
    }
}
