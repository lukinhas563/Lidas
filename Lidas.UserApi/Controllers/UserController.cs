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
        private readonly EmailService _email;

        public UserController(AppDbContext context,
            IMapper mapper,
            TokenService token,
            CryptographyService cryptography,
            UserValidator validator,
            EmailService email)
        {
            _context = context;
            _mapper = mapper;
            _token = token;
            _cryptography = cryptography;
            _validator = validator;
            _email = email;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserInput input)
        {
            // Validator
            var result = await _validator.Register.ValidateAsync(input);
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
                // Email confirm
                var tokenConfirmation = _token.GenerateEmailToken(user);
                var linkConfirmation = Url.Action(nameof(Confirmation), "User", new { tokenConfirmation }, Request.Scheme);

                _email.SendEmail(user.Name, user.Email, "Confirm your email", $"Click on the link to confirm your email: {linkConfirmation}");


                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // View
                var viewModel = _mapper.Map<UserView>(user);

                return Ok(viewModel);
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
        public async Task<IActionResult> Update(Guid id, UserInput input)
        {
            // Validator
            var result = await _validator.Register.ValidateAsync(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var user = _context.Users.SingleOrDefault(user => user.Id == id && !user.IsDeleted);

            if (input == null) return NotFound();

            try
            {
                user.Update(input.Name, input.LastName, input.UserName, input.Email, input.Password);

                _context.Users.Update(user);
                _context.SaveChanges();

                return NoContent();
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> Confirmation(string token)
        {
            var validUser = await _token.ValidateToken(token);

            if (validUser == null) return BadRequest();

            validUser.IsEmailConfirmed = true;

            _context.SaveChanges();

            return NoContent();
        }

    }
}
