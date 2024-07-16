using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/manga")]
    [ApiController]
    public class MangaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MangaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var mangas = _context.Mangas.Where(manga => !manga.IsDeleted).ToList();

            return Ok(mangas);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var manga = _context.Mangas
                .Include(manga => manga.Categories)
                .SingleOrDefault(manga =>  manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            return Ok(manga);
        }

        [HttpPost]
        public IActionResult Create(Manga manga)
        {
            _context.Mangas.Add(manga);

            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new {id = manga.Id}, manga);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Manga input)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id);

            if (manga == null) return NotFound();

            manga.Update(input.Banner, input.Cover, input.Name, input.Description, input.Release);

            _context.Mangas.Update(manga);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            manga.Delete();
            _context.SaveChanges(true);

            return NoContent();
        }

        [HttpPost("{id}/category/{categoryId}")]
        public IActionResult AddCategory(Guid id, Guid categoryId)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var category = _context.Categories.SingleOrDefault(category => category.Id == categoryId && !category.IsDeleted);

            if (category == null) return NotFound();

            manga.Categories.Add(category);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("{id}/author/{authorId}")]
        public IActionResult AddAuthor(Guid id, Guid authorId)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var author = _context.Authors.SingleOrDefault(author => author.Id == authorId && !author.IsDeleted);

            if (author == null) return NotFound();

            manga.Authors.Add(author);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("{id}/chapter/{chapterId}")]
        public IActionResult AddChapter(Guid id, Guid chapterId)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.Id == chapterId && !chapter.IsDeleted);

            if (chapter == null) return NotFound();

            manga.Chapters.Add(chapter);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
