using AutoMapper;
using Lidas.UserApi.Entities;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Models.View;
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
        private readonly IMapper _mapper;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Database
            var users = _context.Users.Where(user => !user.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<UserViewList>>(users);

            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Database
            var user = _context.Users.SingleOrDefault(user => user.Id == id && !user.IsDeleted);

            if (user == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<UserView>(user);

            return Ok(viewModel);
        }

        [HttpPost("register")]
        public IActionResult Register(UserInput input)
        {
            // Mapper
            var user = _mapper.Map<User>(input);

            // Database
            _context.Users.Add(user);
            _context.SaveChanges();

            var viewModel = _mapper.Map<UserView>(user);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, viewModel);
        }

        [HttpPost("login")]
        public IActionResult Login(UserInput input)
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

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UserInput input)
        {
            var user = _context.Users.SingleOrDefault(user => user.Id == id && !user.IsDeleted);

            if (input == null) return NotFound();

            user.Update(input.Name, input.LastName, input.UserName, input.Email, input.Password);

            _context.Users.Update(user);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
