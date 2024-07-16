﻿using AutoMapper;
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
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly RoleValidator _validator;

        public RoleController(AppDbContext context, IMapper mapper, RoleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Database
            var roles = _context.Roles.Where(role => !role.IsDeleted).ToList();

            // Mapper
            var viewModel = _mapper.Map<List<RoleViewList>>(roles);

            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Database
            var role = _context.Roles
                .Include(role => role.Authors)
                .SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            // Mapper
            var viewModel = _mapper.Map<RoleView>(role);

            return Ok(viewModel);
        }

        [HttpPost]
        public IActionResult Create(RoleInput input)
        {
            // Validator
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Mapper
            var role = _mapper.Map<Role>(input);

            // Database
            _context.Roles.Add(role);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, RoleInput input)
        {
            // Validator
            var result = _validator.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var role = _context.Roles.SingleOrDefault(role => role.Id == id);

            if (role == null) return NotFound();

            role.Update(input.Name);
            _context.Roles.Update(role);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var role = _context.Roles.SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            role.Delete();
            _context.SaveChanges();

            return NoContent();
        }
    }
}
