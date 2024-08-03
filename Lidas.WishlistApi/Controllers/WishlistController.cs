using AutoMapper;
using Contracts;
using FluentValidation;
using Lidas.WishlistApi.Database;
using Lidas.WishlistApi.Entities;
using Lidas.WishlistApi.Interfaces;
using Lidas.WishlistApi.Models.Input;
using Lidas.WishlistApi.Models.View;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private readonly IRequestService _request;
        public WishlistController(AppDbContext context, IMapper mapper, IValidatorService validator, IPublishEndpoint publish, IRequestService request)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _publish = publish;
            _request = request;
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            var wishList = await _context.Wishlists
                .Include(wish => wish.Wishitems)
                .SingleOrDefaultAsync(wish => wish.UserId == userId && !wish.IsDeleted);

            if (wishList == null) return NotFound();

            var mangaIds = wishList.Wishitems
                .Where(w => !w.IsDeleted)
                .Select(w => w.MangaId)
                .ToList();

            if (!mangaIds.Any()) return Ok(mangaIds);

            var mangas = await _request.GetAll(mangaIds);

            return Ok(mangas);
        }

        [HttpPost("{userId}/{mangaId}")]
        [Authorize]
        public async Task<IActionResult> AddManga(Guid userId, Guid mangaId)
        {
            // Database
            var wishList = _context.Wishlists.SingleOrDefault(list => list.UserId == userId && !list.IsDeleted);

            if (wishList  == null) return NotFound();

            var wishItem = _context.Wishitems.SingleOrDefault(item => item.MangaId == mangaId && !item.IsDeleted);

            if (wishItem == null) return NotFound();

            // Add manga
            wishList.Wishitems.Add(wishItem);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{userId}/{mangaId}")]
        [Authorize]
        public async Task<IActionResult> RemoveManga(Guid userId, Guid mangaId)
        {
            var wishList = _context.Wishlists
                .Include(list =>  list.Wishitems)
                .SingleOrDefault(list => list.UserId == userId && !list.IsDeleted);

            if (wishList == null) return NotFound();

            var wishItem = wishList.Wishitems.SingleOrDefault(item => item.MangaId == mangaId && !item.IsDeleted);

            if (wishItem == null) return NotFound();

            wishList.Wishitems.Remove(wishItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

}
