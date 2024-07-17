using AutoMapper;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.PageModels;
using Lidas.MangaApi.Models.ViewModels;
using Lidas.MangaApi.Persist;
using Lidas.MangaApi.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly AuthorValidator _validator;
        public AuthorController(AppDbContext context, IMapper mapper, AuthorValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        /// <summary>
        /// Get all available author
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Author collection</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            // Database
            var count = _context.Authors.Count();
            var authors = _context.Authors.Where(author => !author.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<AuthorViewList>>(authors);

            // Pagination
            var pageView = new PageView<AuthorViewList>(page, size, count, viewModel);

            return Ok(pageView);
        }

        /// <summary>
        /// Get one available author
        /// </summary>
        /// <param name="id">Author identifier</param>
        /// <returns>Author object data</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            // Database
            var author = _context.Authors
                .Include(author => author.Roles)
                .Include(author => author.Mangas)
                .SingleOrDefault(author => !author.IsDeleted && author.Id == id);

            if (author == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<AuthorView>(author);

            return Ok(viewModel);
        }

        /// <summary>
        /// Register a new author
        /// </summary>
        /// <param name="input">Author data</param>
        /// <returns>Author object data</returns>
        /// <response code="201">Success</response>
        /// <response code="400">Bad Request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(AuthorInput input)
        {
            // Validate
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var author = _mapper.Map<Author>(input);

            // Database
            _context.Authors.Add(author);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        /// <summary>
        /// Update a author
        /// </summary>
        /// <param name="id">Author identifier</param>
        /// <param name="input">Author data</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(Guid id, AuthorInput input)
        {
            // Validate
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var author = _context.Authors.SingleOrDefault(author => author.Id == id);

            if (author == null) return NotFound();

            author.Update(input.Name, input.Biography, input.Birthday);
            _context.Authors.Update(author);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Delete a auhtor
        /// </summary>
        /// <param name="id">Author identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            var author = _context.Authors.SingleOrDefault(author => author.Id == id && !author.IsDeleted);

            if (author == null) return NotFound();

            author.Delete();
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Add role to a author
        /// </summary>
        /// <param name="id">Author identifier</param>
        /// <param name="roleId">Role identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpPost("{id}/role/{roleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AddRole(Guid id, Guid roleId)
        {
            var author = _context.Authors.SingleOrDefault(author => author.Id == id && !author.IsDeleted);

            if (author == null) return NotFound();

            var role = _context.Roles.SingleOrDefault(role => role.Id == roleId && !role.IsDeleted);

            if (role == null) return NotFound();

            author.Roles.Add(role);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
