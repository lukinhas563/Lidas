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

            // Debugging
            Console.WriteLine("Manga IDs: " + string.Join(", ", mangaIds));

            var mangas = await _request.GetAll(mangaIds);

            // Debugging
            Console.WriteLine("Mangas received: " + (mangas.Count > 0 ? "Not Empty" : "Empty"));

            return Ok(mangas);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddManga(Guid userId, WishitemInput input)
        {
            // Validate
            var result = _validator.Wish.Validate(input);
            var errors = result.Errors.Select(error => error.ErrorMessage);

            if (!result.IsValid) return BadRequest(errors);

            // Database
            var wishList = _context.Wishlists.SingleOrDefault(list => list.UserId == userId && !list.IsDeleted); // User wishList

            if (wishList  == null) return NotFound();

            var wishItem = _context.Wishitems.SingleOrDefault(item => item.MangaId == input.MangaId);

            if (wishItem == null) return NotFound();

            // Add manga
            wishList.Wishitems.Add(wishItem);
            _context.SaveChanges();

            return NoContent();
        }
    }

}
