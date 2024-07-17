using AutoMapper;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.ViewModels;
using Lidas.MangaApi.Persist;
using Lidas.MangaApi.Validators;
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
        private readonly ChapterValidator _validator;
        public ChapterController(AppDbContext context, IMapper mapper, ChapterValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        /// <summary>
        /// Get all available chapters
        /// </summary>
        /// <returns>Chapter collection</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            // Database
            var chapters = _context.Chapters.Where(chapter => !chapter.IsDeleted);

            // Mapper
            var viewModel = _mapper.Map<List<ChapterView>>(chapters);

            return Ok(viewModel);
        }

        /// <summary>
        /// Get one available chapter
        /// </summary>
        /// <param name="id">Chapter identifier</param>
        /// <returns>Chapter object data</returns>
        /// <response code="200">Success</response>
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

        /// <summary>
        /// Register a new chapter
        /// </summary>
        /// <param name="input">Chapter data</param>
        /// <returns>Chapter object data</returns>
        /// <response code="201">Success</response>
        /// <response code="404">Bad Request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(ChapterInput input)
        {
            // Validator
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var chapter = _mapper.Map<Chapter>(input);

            // Database
            _context.Chapters.Add(chapter);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = chapter.Id }, chapter);
        }

        /// <summary>
        /// Update a chapter
        /// </summary>
        /// <param name="id">Chapter identifier</param>
        /// <param name="input">Chapter data</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(Guid id, ChapterInput input)
        {
            // Validator
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.Id == id);

            if (chapter == null) return NotFound();

            chapter.Update(input.Number, input.Title);
            _context.Chapters.Update(chapter);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Delete a chapter
        /// </summary>
        /// <param name="id">Chapter identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            var chapter = _context.Chapters.SingleOrDefault(chapter => chapter.Id == id && !chapter.IsDeleted);

            if (chapter == null) return NotFound();

            chapter.Delete();
            _context.SaveChanges();

            return NoContent();
        }
    }
}
