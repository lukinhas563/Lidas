using AutoMapper;
using FluentValidation;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.ViewModels;
using Lidas.MangaApi.Persist;
using Lidas.MangaApi.Validators;
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
        private readonly MangaValidator _validator;
        public MangaController(AppDbContext context, IMapper mapper, MangaValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        /// <summary>
        /// Get all available manga
        /// </summary>
        /// <returns>Manga colletion</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        /// <summary>
        /// Get one available manga
        /// </summary>
        /// <param name="id">Manga identifier</param>
        /// <returns>Manga object data</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Register a new Manga
        /// </summary>
        /// <param name="input">Manga data</param>
        /// <returns>Manga object data</returns>
        /// <response code="201">Success</response>
        /// <response code="400">Bad Request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(MangaInput input)
        {
            // Validator
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var manga = _mapper.Map<Manga>(input);

            // Database
            _context.Mangas.Add(manga);

            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new {id = manga.Id}, manga);
        }

        /// <summary>
        /// Update a manga
        /// </summary>
        /// <param name="id">Manga identifier</param>
        /// <param name="input">Manga data</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(Guid id, MangaInput input)
        {
            // Validator
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id);

            if (manga == null) return NotFound();

            manga.Update(input.Banner, input.Cover, input.Name, input.Description, input.Release);

            _context.Mangas.Update(manga);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Delete a manga
        /// </summary>
        /// <param name="id">Manga identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            manga.Delete();
            _context.SaveChanges(true);

            return NoContent();
        }

        /// <summary>
        /// Add category to a manga
        /// </summary>
        /// <param name="id">Manga identifier</param>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpPost("{id}/category/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Add author to a manga
        /// </summary>
        /// <param name="id">Manga identifier</param>
        /// <param name="authorId">Author identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpPost("{id}/author/{authorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Add chapter to a manga
        /// </summary>
        /// <param name="id">Manga identifier</param>
        /// <param name="chapterId">Chapter identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpPost("{id}/chapter/{chapterId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
