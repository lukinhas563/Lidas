using AutoMapper;
using Lidas.UserApi.Entities;
using Lidas.UserApi.Models.Input;
using Lidas.UserApi.Models.View;
using Lidas.UserApi.Persist;
using Lidas.UserApi.Services;
using Lidas.UserApi.Validations;
using Lidas.UserApi.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.ObjectModel;

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
        private readonly UserValidator _validator;

        public UserController(AppDbContext context,
            IMapper mapper,
            TokenService token,
            CryptographyService cryptography,
            UserValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _token = token;
            _cryptography = cryptography;
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserInput input)
        {
            // Validator
            var result = _validator.Register.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Initial role
            var role = _context.Roles.SingleOrDefault(role => role.Name == "Basic" && !role.IsDeleted);

            if (role == null) return NotFound();

            // Mapper
            var hashPassword = _cryptography.Hash(input.Password);
            input.Password = hashPassword;

            var user = _mapper.Map<User>(input);
            user.Role = role;

            // Database
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var viewModel = _mapper.Map<UserView>(user);

                return Ok(viewModel);
            } 
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {

                var err = new List<string>();

                if (pgEx.ConstraintName.Contains("IX_Users_UserName"))
                {
                    err.Add("Username already exists.");
                }

                if (pgEx.ConstraintName.Contains("IX_Users_Email"))
                {
                    err.Add("Email already exists.");
                }

                Console.WriteLine(err);

                return BadRequest(err);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }

        }

        [HttpPost("login")]
        public IActionResult Login(LoginInput input)
        {
            // Validator
            var result = _validator.Login.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var user = _context.Users
                .Include(user => user.Role)
                .SingleOrDefault(user => user.UserName == input.UserName && !user.IsDeleted);

            if (user == null) return BadRequest("Username or password is not correct.");
            if (!user.IsEmailConfirmed) return BadRequest("Email was not confirmed.");

            // Cryptography
            var isValid = _cryptography.Verify(user.Password, input.Password);

            if (!isValid) return BadRequest("Username or password is not correct.");

            var token = _token.GenerateToken(user);

            return Ok(token);
      
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UserInput input)
        {
            var user = _context.Users.SingleOrDefault(user => user.Id == id && !user.IsDeleted);

            if (input == null) return NotFound();

            // Database
            try
            {
                user.Update(input.Name, input.LastName, input.UserName, input.Email, input.Password);

                _context.Users.Update(user);
                _context.SaveChanges();

                return NoContent();
            } 
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                var err = new List<string>();

                if (pgEx.ConstraintName.Contains("IX_Users_UserName"))
                {
                    err.Add("Username already exists.");
                }

                if (pgEx.ConstraintName.Contains("IX_Users_Email"))
                {
                    err.Add("Email already exists.");
                }

                Console.WriteLine(err);

                return BadRequest(err);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost("{id}")]
        public IActionResult Confirmation(Guid id)
        {
            var user = _context.Users.SingleOrDefault(user => user.Id == id && !user.IsDeleted);

            if (user == null) return NotFound();

            user.IsEmailConfirmed = true;

            _context.SaveChanges();

            return NoContent();
        }
    }
}
