using AutoMapper;
using Lidas.UserApi.Entities;
using Lidas.UserApi.Interfaces;
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
        private readonly IToken _token;
        private readonly ICryptography _cryptography;
        private readonly IValidatorService _validator;
        private readonly IEmail _email;

        public UserController(AppDbContext context,
            IMapper mapper,
            IToken token,
            ICryptography cryptography,
            IValidatorService validator,
            IEmail email)
        {
            _context = context;
            _mapper = mapper;
            _token = token;
            _cryptography = cryptography;
            _validator = validator;
            _email = email;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="input">User data</param>
        /// <returns>Object user data</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

                // Save
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

        /// <summary>
        /// Log in a user
        /// </summary>
        /// <param name="input">User connect data</param>
        /// <returns>Object token</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <param name="input">User data</param>
        /// <returns>No return</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Confirm an e-mail
        /// </summary>
        /// <param name="token">Token data</param>
        /// <returns>No return</returns>
        /// <response data="204">Success</response>
        /// <response data="400">Bad Request</response>
        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Confirmation(string token)
        {
            var validUser = await _token.ValidateEmailToken(token);

            if (validUser == null) return BadRequest();

            validUser.IsEmailConfirmed = true;

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Send a password link reset
        /// </summary>
        /// <param name="emailInput">Email data</param>
        /// <returns>No return</returns>
        /// <response data="204">Success</response>
        /// <response data="400">Bad Request</response>
        /// <response data="500">Internal Server Error</response>
        [HttpPost("request-password-reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RequestPasswordReset(EmailInput emailInput)
        {
            // Validator
            var result = _validator.Email.Validate(emailInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var user = _context.Users.SingleOrDefault(user => user.Email == emailInput.Email && !user.IsDeleted);

            if (user == null) return NotFound();

            // Token reset
            try
            {
                var resetToken = _token.GeneratePasswordToken(user);
                var resetLink = Url.Action(nameof(ResetPassword), "User", new { token = resetToken }, Request.Scheme);

                _email.SendEmail(user.Name, user.Email, "Password Reset Request", $"Click the link to reset your password: {resetLink}");

                return NoContent();
            } 
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }

        }

        /// <summary>
        /// Confirm a token and reset user's password
        /// </summary>
        /// <param name="passwordInput">Password data</param>
        /// <param name="token">Token data</param>
        /// <returns>No return</returns>
        /// <response data="204">Success</response>
        /// <response data="400">Bad Request</response>
        [HttpPost("password-reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(PasswordInput passwordInput, string token)
        {
            // Validator
            var validUser = await _token.ValidadePasswordToken(token);

            if (validUser == null) return BadRequest();

            var result = _validator.Password.Validate(passwordInput);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            validUser.Password = _cryptography.Hash(passwordInput.Password);
            await _context.SaveChangesAsync();

            return NoContent();

        }

    }
}
