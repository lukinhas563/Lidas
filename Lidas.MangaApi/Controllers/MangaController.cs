using AutoMapper;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.ViewModels;
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
        private readonly IMapper _mapper;
        public MangaController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Database
            var mangas = _context.Mangas
                .Include(manga => manga.Categories)
                .Where(manga => !manga.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<MangaViewList>>(mangas);

            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Database
            var manga = _context.Mangas
                .Include(manga => manga.Categories)
                .Include(manga => manga.Authors)
                .Include(manga => manga.Chapters)
                .SingleOrDefault(manga =>  manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<MangaView>(manga);

            return Ok(viewModel);
        }

        [HttpPost]
        public IActionResult Create(MangaInput input)
        {
            // Mapper
            var manga = _mapper.Map<Manga>(input);

            // Database
            _context.Mangas.Add(manga);

            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new {id = manga.Id}, manga);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, MangaInput input)
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
