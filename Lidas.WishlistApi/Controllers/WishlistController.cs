using Lidas.WishlistApi.Database;
using Lidas.WishlistApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.WishlistApi.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;
        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var wishes = _context.Wishes.Where(wish => !wish.IsDeleted).ToList();

            return Ok(wishes);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var wish = _context.Wishes.SingleOrDefault(wish => wish.Id == id && !wish.IsDeleted);

            if (wish == null) return NotFound();

            return Ok(wish);
        }

        [HttpPost]
        public IActionResult Create(Wish wish)
        {
            _context.Add(wish);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Wish input)
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
