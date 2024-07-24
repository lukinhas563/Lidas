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
using System;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;

        public CategoryController(AppDbContext context, IMapper mapper, IValidatorService validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        /// <summary>
        /// Get all available categories
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Category collection</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            // Database
            var count = _context.Categories.Count();
            var categories = _context.Categories.Where(category => !category.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<CategoryViewList>>(categories);

            // Pagination
            var pageView = new PageView<CategoryViewList>(page, size, count, viewModel);
            
            return Ok(pageView);
        }

        /// <summary>
        /// Get one available category
        /// </summary>
        /// <param name="id">Category identifier</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Category object data</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            // Database
            var category = _context.Categories
                .Include(category => category.Mangas)
                .SingleOrDefault(category => category.Id == id && !category.IsDeleted);

            if (category == null) return NotFound();

            // Mapper Manga
            var count = category.Mangas.Count();
            var mangaPages = category.Mangas.Skip(page).Take(size).ToList();

            var mangaView = _mapper.Map<List<MangaViewList>>(mangaPages);
            var mangaPageView = new PageView<MangaViewList>(page, size, count, mangaView);

            // Mapper Category
            var viewModel = _mapper.Map<CategoryView>(category);
            viewModel.Mangas = mangaPageView;

            return Ok(viewModel);
        }

        /// <summary>
        /// Register a new category
        /// </summary>
        /// <param name="input">Category data</param>
        /// <returns>Category object data</returns>
        /// <response code="201">Success</response>
        /// <response code="400">Bad Request</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(CategoryInput input)
        {
            // Validate
            var result = _validator.Category.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var category = _mapper.Map<Category>(input);

            // Database
            _context.Categories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Upadte a category
        /// </summary>
        /// <param name="id">Category identifier</param>
        /// <param name="input">Category data</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(Guid id, CategoryInput input)
        {
            // Validate
            var result = _validator.Category.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var category = _context.Categories.SingleOrDefault(category => category.Id == id);

            if (category == null) return NotFound();

            category.Update(input.Name);

            _context.Categories.Update(category);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category identifier</param>
        /// <returns>No return</returns>
        /// <reponse code="204">Success</reponse>
        /// <reponse code="404">Not Found</reponse>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            var category = _context.Categories.SingleOrDefault(category => category.Id == id && !category.IsDeleted);

            if (category == null) return NotFound();

            category.Delete();
            _context.SaveChanges();

            return NoContent();
        }

    }
}
