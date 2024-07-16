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
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly CategoryValidator _validator;

        public CategoryController(AppDbContext context, IMapper mapper, CategoryValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Database
            var categories = _context.Categories.Where(category => !category.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<CategoryViewList>>(categories);

            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Database
            var category = _context.Categories
                .Include(category => category.Mangas)
                .SingleOrDefault(category => category.Id == id && !category.IsDeleted);

            if (category == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<CategoryView>(category);

            return Ok(viewModel);
        }

        [HttpPost]
        public IActionResult Create(CategoryInput input)
        {
            // Validate
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var category = _mapper.Map<Category>(input);

            // Database
            _context.Categories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, CategoryInput input)
        {
            // Validate
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
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
