using AutoMapper;
using Lidas.UserApi.Entities;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Models.View;
using Lidas.UserApi.Persist;
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
        public RoleController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        public IActionResult Create(RoleInput input)
        {
            // Mapper
            var role = _mapper.Map<Role>(input);

            // Database
            try
            {
                _context.Roles.Add(role);
                _context.SaveChanges();

                // Viewr
                var viewModel = _mapper.Map<RoleView>(role);

                return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
            } 
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                if (pgEx.ConstraintName.Contains("IX_Roles_Name"))
                {
                    return BadRequest("Role already exists.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, RoleInput input)
        {
            var role = _context.Roles.SingleOrDefault(role => role.Id == id);

            if (role == null) return NotFound();

            // Database
            try
            {
                role.Update(input.Name);

                _context.Roles.Update(role);
                _context.SaveChanges();

                return NoContent();

            } 
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                if (pgEx.ConstraintName.Contains("IX_Roles_Name"))
                {
                    return BadRequest("Role already exists.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
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
