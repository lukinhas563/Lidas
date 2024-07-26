using AutoMapper;
using Contracts;
using Lidas.WishlistApi.Database;
using Lidas.WishlistApi.Entities;
using Lidas.WishlistApi.Interfaces;
using Lidas.WishlistApi.Models.Input;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.WishlistApi.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;
        private readonly IPublishEndpoint _publish;
        public WishlistController(AppDbContext context, IMapper mapper, IValidatorService validator, IPublishEndpoint publish)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _publish = publish;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var wishes = _context.Wishes.Where(wish => !wish.IsDeleted).ToList();

            var wishview = _mapper.Map<List<Wish>>(wishes);

            return Ok(wishview);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var wish = _context.Wishes.SingleOrDefault(wish => wish.Id == id && !wish.IsDeleted);

            if (wish == null) return NotFound();

            await _publish.Publish(new MangaRequestEvent
            {
                Id = wish.MangaId,
                RequestedAt = DateTime.UtcNow,
            });

            var wishView = _mapper.Map<Wish>(wish);

            return Ok(wishView);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(WishInput input)
        {
            // Validator
            var result = _validator.Wish.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var wish = _mapper.Map<Wish>(input);

            _context.Add(wish);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(Guid id, WishInput input)
        {
            // Validator
            var result = _validator.Wish.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var wish = _context.Wishes.SingleOrDefault(wish => wish.Id == id);

            if (wish == null) return NotFound();

            wish.Update(input.UserId, input.MangaId);

            _context.Update(wish);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            var wish = _context.Wishes.SingleOrDefault(wish => wish.Id == id && !wish.IsDeleted);

            if (wish == null) return NotFound();

            wish.Delete();
            _context.SaveChanges();

            return NoContent();
        }
    }

}
