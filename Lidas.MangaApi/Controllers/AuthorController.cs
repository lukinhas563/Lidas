using AutoMapper;
using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Models.InputModels;
using Lidas.MangaApi.Models.ViewModels;
using Lidas.MangaApi.Persist;
using Lidas.MangaApi.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly AuthorValidator _validator;
        public AuthorController(AppDbContext context, IMapper mapper, AuthorValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Database
            var authors = _context.Authors.Where(author => !author.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<AuthorViewList>>(authors);

            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Database
            var author = _context.Authors
                .Include(author => author.Roles)
                .Include(author => author.Mangas)
                .SingleOrDefault(author => !author.IsDeleted && author.Id == id);

            if (author == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<AuthorView>(author);

            return Ok(viewModel);
        }

        [HttpPost]
        public IActionResult Create(AuthorInput input)
        {
            // Validate
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var author = _mapper.Map<Author>(input);

            // Database
            _context.Authors.Add(author);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, AuthorInput input)
        {
            // Validate
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var author = _context.Authors.SingleOrDefault(author => author.Id == id);

            if (author == null) return NotFound();

            author.Update(input.Name, input.Biography, input.Birthday);
            _context.Authors.Update(author);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var author = _context.Authors.SingleOrDefault(author => author.Id == id && !author.IsDeleted);

            if (author == null) return NotFound();

            author.Delete();
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("{id}/role/{roleId}")]
        public IActionResult AddRole(Guid id, Guid roleId)
        {
            var author = _context.Authors.SingleOrDefault(author => author.Id == id && !author.IsDeleted);

            if (author == null) return NotFound();

            var role = _context.Roles.SingleOrDefault(role => role.Id == roleId && !role.IsDeleted);

            if (role == null) return NotFound();

            author.Roles.Add(role);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
