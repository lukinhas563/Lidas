using Lidas.LikeApi.Database;
using Lidas.LikeApi.Interfaces;
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
        private readonly IRequestService _request;
        public LikeController(AppDbContext context, IRequestService request)
        {
            _context = context;
            _request = request;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            var likeList = _context.Likelists
                .Include(list => list.Likeitems)
                .SingleOrDefault(list => list.UserId == userId && !list.IsDeleted);

            if (likeList == null) return NotFound();

            var mangaIds = likeList.Likeitems
                .Where(item => !item.IsDeleted)
                .Select(item => item.MangaId).ToList();

            if (!mangaIds.Any()) return Ok(mangaIds);

            var mangas = await _request.GetAll(mangaIds);

            return Ok(mangas);
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
