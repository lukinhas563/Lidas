using AutoMapper;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.ViewModels;
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
        private readonly IMapper _mapper;

        public ChapterController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Database
            var chapters = _context.Chapters.Where(chapter => !chapter.IsDeleted);

            // Mapper
            var viewModel = _mapper.Map<List<ChapterView>>(chapters);

            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Database
            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.Id == id && !chapter.IsDeleted);

            if (chapter == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<ChapterView>(chapter);

            return Ok(viewModel);
        }

        [HttpPost]
        public IActionResult Create(ChapterInput input)
        {
            // Mapper
            var chapter = _mapper.Map<Chapter>(input);

            // Database
            _context.Chapters.Add(chapter);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = chapter.Id }, chapter);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, ChapterInput input)
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
