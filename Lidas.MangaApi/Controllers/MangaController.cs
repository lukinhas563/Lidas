using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var manga = _context.Mangas.SingleOrDefault(manga =>  manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            return Ok(manga);
        }

        [HttpPost]
        public IActionResult Create(Manga manga)
        {
            _context.Mangas.Add(manga);

            return CreatedAtAction(nameof(GetById), new {id = manga.Id}, manga);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Manga input)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id);

            if (manga == null) return NotFound();

            manga.Update(input.Banner, input.Cover, input.Name, input.Description, input.Release);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            manga.Delete();

            return NoContent();
        }
    }
}
