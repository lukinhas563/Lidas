using Lidas.LikeApi.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lidas.LikeApi.Controllers
{
    [Route("api/like")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LikeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public IActionResult GetAll(Guid userId)
        {
            var likeList = _context.Likelists
                .Include(list => list.Likeitems)
                .SingleOrDefault(list => list.UserId == userId && !list.IsDeleted);

            if (likeList == null) return NotFound();

            return Ok(likeList);
        }

        [HttpPost("{userId}/{mangaId}")]
        public IActionResult Like(Guid userId, Guid mangaId)
        {
            var likeList = _context.Likelists.SingleOrDefault(list => list.UserId == userId && !list.IsDeleted);

            if (likeList == null) return NotFound();

            var likeItem = _context.Likeitems.SingleOrDefault(item => item.MangaId == mangaId && !item.IsDeleted);

            if (likeItem == null) return NotFound();

            likeList.Likeitems.Add(likeItem);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{userId}/{mangaId}")]
        public IActionResult Remove(Guid userId, Guid mangaId)
        {
            var likeList = _context.Likelists.SingleOrDefault(list => list.UserId == userId && !list.IsDeleted);

            if (likeList == null) return NotFound();

            var likeItem = _context.Likeitems.SingleOrDefault(item => item.MangaId == mangaId && !item.IsDeleted);

            if (likeItem == null) return NotFound();

            likeList.Likeitems.Remove(likeItem);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
