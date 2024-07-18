using Lidas.UserApi.Entities;
using Lidas.UserApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.UserApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users.Where(user => !user.IsDeleted).ToList();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user = _context.Users.SingleOrDefault(user => user.Id == id && !user.IsDeleted);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPost("register")]
        public IActionResult Register(User input)
        {
            _context.Users.Add(input);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        [HttpPost("login")]
        public IActionResult Login(User input)
        {
            var user = _context.Users.SingleOrDefault(user => !user.IsDeleted && user.UserName == input.UserName);

            if (user == null) return NotFound();

            if (user.Password ==  input.Password)
            {
                return Ok("Valid");
            } 
            else
            {
                return BadRequest("Invalid");
            }
        }
    }
}
