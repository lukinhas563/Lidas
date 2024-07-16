using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var authors = _context.Authors.Where(author => !author.IsDeleted).ToList();

            return Ok(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var author = _context.Authors.SingleOrDefault(author => !author.IsDeleted && author.Id == id);

            if (author == null) return NotFound();

            return Ok(author);
        }

        [HttpPost]
        public IActionResult Create(Author author)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Author input)
        {
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
