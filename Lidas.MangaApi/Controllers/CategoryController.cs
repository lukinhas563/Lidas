using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _context.Categories.Where(category => !category.IsDeleted).ToList();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var category = _context.Categories.SingleOrDefault(category => category.Id == id && !category.IsDeleted);

            if (category == null) return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Category input)
        {
            var category = _context.Categories.SingleOrDefault(category => category.Id == id);

            if (category == null) return NotFound();

            category.Update(input.Name);

            _context.Categories.Update(category);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid mangaId, Guid id)
        {
            var category = _context.Categories.SingleOrDefault(category => category.Id == id && !category.IsDeleted);

            if (category == null) return NotFound();

            category.Delete();
            _context.SaveChanges();

            return NoContent();
        }

    }
}
