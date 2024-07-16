using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/chapter")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChapterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var chapters = _context.Chapters.Where(chapter => !chapter.IsDeleted);

            return Ok(chapters);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.Id == id && !chapter.IsDeleted);

            if (chapter == null) return NotFound();

            return Ok(chapter);
        }

        [HttpPost]
        public IActionResult Create(Chapter chapter)
        {
            _context.Chapters.Add(chapter);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = chapter.Id }, chapter);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Chapter input)
        {
            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.Id == id);

            if (chapter == null) return NotFound();

            chapter.Update(input.Number, input.Title);
            _context.Chapters.Update(chapter);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid mangaId, Guid id)
        {
            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.Id == id && !chapter.IsDeleted);

            if (chapter == null) return NotFound();

            chapter.Delete();
            _context.SaveChanges();

            return NoContent();
        }
    }
}
