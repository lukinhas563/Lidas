using AutoMapper;
using Lidas.WishlistApi.Database;
using Lidas.WishlistApi.Entities;
using Lidas.WishlistApi.Models.Input;
using MassTransit;
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
        public WishlistController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var wishes = _context.Wishes.Where(wish => !wish.IsDeleted).ToList();

            var wishview = _mapper.Map<List<Wish>>(wishes);

            return Ok(wishview);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var wish = _context.Wishes.SingleOrDefault(wish => wish.Id == id && !wish.IsDeleted);

            if (wish == null) return NotFound();

            var wishView = _mapper.Map<Wish>(wish);

            return Ok(wishView);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WishInput input)
        {
            var wish = _mapper.Map<Wish>(input);

            _context.Add(wish);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, WishInput input)
        {
            var wish = _context.Wishes.SingleOrDefault(wish => wish.Id == id);

            if (wish == null) return NotFound();

            wish.Update(input.UserId, input.MangaId);

            _context.Update(wish);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
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
