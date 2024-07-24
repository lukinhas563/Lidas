using AutoMapper;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Interfaces;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.PageModels;
using Lidas.MangaApi.Models.ViewModels;
using Lidas.MangaApi.Persist;
using Lidas.MangaApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/chapter")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;
        public ChapterController(AppDbContext context, IMapper mapper, IValidatorService validator)
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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll
            (
            [FromQuery] int page = 0,
            [FromQuery] int size = 10,
            [FromQuery] string sortOrder = "desc",
            [FromQuery] string name = null
            )
        {
            if (sortOrder != "desc" && sortOrder != "asc")
            {
                return BadRequest("Invalid sortOrder parameter. Use 'asc' for ascending or 'desc' for descending.");
            }

            // Database
            var queryCount = _context.Chapters.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                var namePattern = $"%{name}%";
                queryCount = queryCount.Where(chapter => EF.Functions.Like(chapter.Title, namePattern));
            }

            var count = queryCount.Count();

            IQueryable<Chapter> query = queryCount.Where(chapter => !chapter.IsDeleted);

            if (sortOrder == "asc")
            {
                query = query.OrderBy(chapter => chapter.CreatedAt);
            }
            else
            {
                query = query.OrderByDescending(chapter => chapter.CreatedAt);
            }

            var chapters = query.Skip(page).Take(size).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<ChapterView>>(chapters);

            // Pagination
            var pageView = new PageView<ChapterView>(page, size, count, viewModel);

            return Ok(pageView);
        }

        /// <summary>
        /// Get one available chapter
        /// </summary>
        /// <param name="id">Chapter identifier</param>
        /// <returns>Chapter object data</returns>
        /// <response code="200">Success</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(ChapterInput input)
        {
            // Validator
            var result = _validator.Chapter.Validate(input);
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(Guid id, ChapterInput input)
        {
            // Validator
            var result = _validator.Chapter.Validate(input);
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
        [Authorize(Roles = "Admin")]
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
