using AutoMapper;
using Lidas.UserApi.Entities;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Models.View;
using Lidas.UserApi.Persist;
using Lidas.UserApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lidas.UserApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly TokenService _token;
        private readonly CryptographyService _cryptography;

        public UserController(AppDbContext context, IMapper mapper, TokenService token, CryptographyService cryptography)
        {
            _context = context;
            _mapper = mapper;
            _token = token;
            _cryptography = cryptography;
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
            var user = _context.Users
                .Include(user => user.Roles)
                .SingleOrDefault(user => user.Id == id && !user.IsDeleted);

            if (user == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<UserView>(user);

            return Ok(viewModel);
        }

        [HttpPost("register")]
        public IActionResult Register(UserInput input)
        {
            // Initial role
            var role = _context.Roles.SingleOrDefault(role => role.Name == "Basic" && !role.IsDeleted);

            if (role == null) return NotFound();

            // Mapper
            var hashPassword = _cryptography.Hash(input.Password);
            input.Password = hashPassword;

            var user = _mapper.Map<User>(input);
            user.Roles.Add(role);

            // Database
            _context.Users.Add(user);
            _context.SaveChanges();

            var viewModel = _mapper.Map<UserView>(user);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, viewModel);
        }

        [HttpPost("login")]
        public IActionResult Login(UserInput input)
        {
            // Database
            var user = _context.Users
                .Include(user => user.Roles)
                .SingleOrDefault(user => !user.IsDeleted && user.UserName == input.UserName);

            if (user == null) return NotFound("Username or password is not correct.");

            // Cryptography
            var result = _cryptography.Verify(user.Password, input.Password);

            if (!result) return BadRequest("Username or password is not correct.");

            var token = _token.GenerateToken(user);

            return Ok(token);
      
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
