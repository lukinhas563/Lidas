using AutoMapper;
using FluentValidation;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Interfaces;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.PageModels;
using Lidas.MangaApi.Models.ViewModels;
using Lidas.MangaApi.Persist;
using Lidas.MangaApi.Services;
using Lidas.MangaApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/manga")]
    [ApiController]
    public class MangaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;
        private readonly IProvider _provider;
        public MangaController
            (
            AppDbContext context,
            IMapper mapper,
            IValidatorService validator,
            IProvider provider
            )
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _provider = provider;
        }

        /// <summary>
        /// Get all available manga
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Manga colletion</returns>
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
            var countQuery = _context.Mangas.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                var namePattern = $"%{name}%";
                countQuery = countQuery.Where(manga => EF.Functions.Like(manga.Name, namePattern));
            }

            var count = countQuery.Count();

            IQueryable<Manga> query = countQuery
                .Include(manga => manga.Categories)
                .Where(manga => !manga.IsDeleted);

            if (sortOrder == "asc")
            {
                query = query.OrderBy(manga => manga.CreatedAt);
            } 
            else
            {
                query = query.OrderByDescending(manga => manga.CreatedAt);
            }

            var mangas = query.Skip(page).Take(size).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<MangaViewList>>(mangas);

            // Pagination
            var pageView = new PageView<MangaViewList>(page, size, count, viewModel);

            return Ok(pageView);
        }

        /// <summary>
        /// Get one available manga
        /// </summary>
        /// <param name="id">Manga identifier</param>
        /// <param name="page">Page number</param>
        /// <param name="size">page size</param>
        /// <returns>Manga object data</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            // Database
            var manga = _context.Mangas
                .Include(manga => manga.Categories)
                .Include(manga => manga.Authors)
                .Include(manga => manga.Chapters)
                .SingleOrDefault(manga =>  manga.Id == id && !manga.IsDeleted);

            if (manga == null) return NotFound();

            // Mapper chapters
            var count = manga.Chapters.Count();
            var chapterPages = manga.Chapters.Skip(page).Take(size).ToList();

            var chapterView = _mapper.Map<List<ChapterView>>(manga.Chapters);
            var chapterPageView = new PageView<ChapterView>(page, size, count, chapterView);

            // Mapper Manga
            var viewModel = _mapper.Map<MangaView>(manga);
            viewModel.Chapters = chapterPageView;

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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(MangaInput input)
        {
            // Validator
            var result = _validator.Manga.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            try
            {
                // Image upload
                var bannerUrl = await _provider.UploadImage(input.Banner);
                var coverUrl = await _provider.UploadImage(input.Cover);

                // Mapper
                var manga = new Manga(bannerUrl, coverUrl, input.Name, input.Description, input.Release);

                // Database
                _context.Mangas.Add(manga);

                _context.SaveChanges();


                return CreatedAtAction(nameof(GetById), new { id = manga.Id }, manga);
            } 
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, MangaInput input)
        {
            // Validator
            var result = _validator.Manga.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == id);

            if (manga == null) return NotFound();

            try
            {
                // Image upload
                var banner = await _provider.UploadImage(input.Banner);
                var cover = await _provider.UploadImage(input.Cover);

                // Database
                manga.Update(banner, cover, input.Name, input.Description, input.Release);

                _context.Mangas.Update(manga);
                _context.SaveChanges();

                return NoContent();
            } 
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Delete a manga
        /// </summary>
        /// <param name="id">Manga identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
