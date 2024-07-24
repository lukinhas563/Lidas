using AutoMapper;
using Lidas.UserApi.Entities;
using Lidas.UserApi.Interfaces;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Models.View;
using Lidas.UserApi.Persist;
using Lidas.UserApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Lidas.UserApi.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase 
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;
        public RoleController(AppDbContext context, IMapper mapper, IValidatorService validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        /// <summary>
        /// Get all available roles
        /// </summary>
        /// <returns>Role object data list</returns>
        /// <response data="200">Success</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            // Database
            var roles = _context.Roles.Where(role => !role.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<RoleViewList>>(roles);

            return Ok(viewModel);
        }

        /// <summary>
        /// Get one available role
        /// </summary>
        /// <param name="id">Role identifier</param>
        /// <returns>Role object data</returns>
        /// <response data="200">Success</response>
        /// <response data="404">Not Found</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            // Databse
            var role = _context.Roles
                .Include(role => role.Users)
                .SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<RoleView>(role);

            return Ok(viewModel);
        }

        /// <summary>
        /// Crate a new role
        /// </summary>
        /// <param name="input">Role data</param>
        /// <returns>Role object data</returns>
        /// <response data="201">Success</response>
        /// <response data="400">Bad Request</response>
        /// <response data="500">Internal Server Error</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(RoleInput input)
        {
            // Validate
            var result = await _validator.Role.ValidateAsync(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var role = _mapper.Map<Role>(input);

            // Database
            try
            {
                _context.Roles.Add(role);
                _context.SaveChanges();

                // View
                var viewModel = _mapper.Map<RoleView>(role);

                return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Update a role
        /// </summary>
        /// <param name="id">Role indentifier</param>
        /// <param name="input">Role data</param>
        /// <returns>No return</returns>
        /// <response data="204">Success</response>
        /// <response data="404">Not Foundt</response>
        /// <response data="400">Bad Request</response>
        /// <response data="500">Internal Server Error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, RoleInput input)
        {
            // Validate
            var result = await _validator.Role.ValidateAsync(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var role = _context.Roles.SingleOrDefault(role => role.Id == id);

            if (role == null) return NotFound();

            try
            {
                role.Update(input.Name);

                _context.Roles.Update(role);
                _context.SaveChanges();

                return NoContent();

            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="id">Role identifier</param>
        /// <returns>No return</returns>
        /// <response data="204">Success</response>
        /// <response data="404">Not Found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            var role = _context.Roles.SingleOrDefault(role => !role.IsDeleted && role.Id == id);

            if (role == null) return NotFound();

            role.Delete();

            _context.SaveChanges();

            return NoContent();
        }
    }
}
