using Lidas.UserApi.Entities;
using Lidas.UserApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.UserApi.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var roles = _context.Roles.Where(role => !role.IsDeleted);

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var role = _context.Roles.SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            return Ok(role);
        }

        [HttpPost]
        public IActionResult Create(Role input)
        {
            _context.Roles.Add(input);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new {id = input.Id}, input);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Role input)
        {
            var role = _context.Roles.SingleOrDefault(role => role.Id == id);

            if (role == null) return NotFound();

            role.Update(input.Name);
            _context.Roles.Update(role);
            _context.SaveChanges();

            return NoContent();
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
