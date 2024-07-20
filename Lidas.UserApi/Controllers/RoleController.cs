using AutoMapper;
using Lidas.UserApi.Entities;
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
        private readonly RoleValidator _validator;
        public RoleController(AppDbContext context, IMapper mapper, RoleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Database
            var roles = _context.Roles.Where(role => !role.IsDeleted);

            // Mapper
            var viewModel = _mapper.Map<List<RoleViewList>>(roles);

            return Ok(viewModel);
        }

        [HttpGet("{id}")]
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

        [HttpPost]
        public async Task<IActionResult> Create(RoleInput input)
        {
            // Validate
            var result = await _validator.ValidateAsync(input);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, RoleInput input)
        {
            // Validate
            var result = await _validator.ValidateAsync(input);
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

        [HttpDelete("{id}")]
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
