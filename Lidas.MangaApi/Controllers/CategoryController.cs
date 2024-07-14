using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/manga/{mangaId}/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(Guid mangaId)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var categories = manga.Categories;

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid mangaId, Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var category = manga.Categories.SingleOrDefault(category => category.Id == id && !category.IsDeleted);

            if (category == null) return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public IActionResult Create(Guid mangaId, Category category)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            manga.Categories.Add(category);

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid mangaId, Guid id, Category input)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var category = manga.Categories.SingleOrDefault(category => category.Id == id);

            if (category == null) return NotFound();

            category.Update(input.Name);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid mangaId, Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var category = manga.Categories.SingleOrDefault(category => category.Id == id);

            if (category == null) return NotFound();

            category.Delete();

            return NoContent();
        }
    }
}
