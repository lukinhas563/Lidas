using AutoMapper;
using Lidas.MangaApi.Entities;
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
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly RoleValidator _validator;

        public RoleController(AppDbContext context, IMapper mapper, RoleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        /// <summary>
        /// Get all available roles
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Role collection</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            // Database
            var count = _context.Roles.Count();
            var roles = _context.Roles.Where(role => !role.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<RoleViewList>>(roles);

            // Pagination
            var pageView = new PageView<RoleViewList>(page, size, count, viewModel);

            return Ok(pageView);
        }

        /// <summary>
        /// Get one available role
        /// </summary>
        /// <param name="id">Role identifier</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Role object data</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            // Database
            var role = _context.Roles
                .Include(role => role.Authors)
                .SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            // Mapper Authors
            var count = role.Authors.Count();
            var authorPage = role.Authors.Skip(page).Take(size).ToList();

            var authorView = _mapper.Map<List<AuthorViewList>>(authorPage);
            var authorPageView = new PageView<AuthorViewList>(page, size, count, authorView);

            // Mapper Roles
            var viewModel = _mapper.Map<RoleView>(role);
            viewModel.Authors = authorPageView;

            return Ok(viewModel);
        }

        /// <summary>
        /// Register a new role
        /// </summary>
        /// <param name="input">Role data</param>
        /// <returns>Role object data</returns>
        /// <response code="201">Success</response>
        /// <response code="400">Bad Request</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(RoleInput input)
        {
            // Validator
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var role = _mapper.Map<Role>(input);

            // Database
            _context.Roles.Add(role);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        /// <summary>
        /// Update a role
        /// </summary>
        /// <param name="id">Role identifier</param>
        /// <param name="input">Role data</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(Guid id, RoleInput input)
        {
            // Validator
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var role = _context.Roles.SingleOrDefault(role => role.Id == id);

            if (role == null) return NotFound();

            role.Update(input.Name);
            _context.Roles.Update(role);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="id">Role identifier</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            var role = _context.Roles.SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            role.Delete();
            _context.SaveChanges();

            return NoContent();
        }
    }
}
