using Lidas.MangaApi.Entities;
using Lidas.MangaApi.Persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lidas.MangaApi.Controllers
{
    [Route("api/manga/{mangaId}/Author/{authorId}/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(Guid mangaId, Guid authorId)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if(manga == null) return NotFound();

            var author = manga.Authors.SingleOrDefault(author => author.Id == authorId && !author.IsDeleted);

            if(author == null) return NotFound();

            var roles = author.Roles;

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid mangaId, Guid authorId, Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var author = manga.Authors.SingleOrDefault(author => author.Id == authorId && !author.IsDeleted);

            if (author == null) return NotFound();

            var role = author.Roles.SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            return Ok(role);
        }

        [HttpPost]
        public IActionResult Create(Guid mangaId, Guid authorId, Role role)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var author = manga.Authors.SingleOrDefault(author => author.Id == authorId && !author.IsDeleted);

            if (author == null) return NotFound();

            author.Roles.Add(role);

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid mangaId, Guid authorId, Guid id, Role input)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var author = manga.Authors.SingleOrDefault(author => author.Id == authorId && !author.IsDeleted);

            if (author == null) return NotFound();

            var role = author.Roles.SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            role.Update(input.Name);

            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete(Guid mangaId, Guid authorId, Guid id)
        {
            var manga = _context.Mangas.SingleOrDefault(manga => manga.Id == mangaId && !manga.IsDeleted);

            if (manga == null) return NotFound();

            var author = manga.Authors.SingleOrDefault(author => author.Id == authorId && !author.IsDeleted);

            if (author == null) return NotFound();

            var role = author.Roles.SingleOrDefault(role => role.Id == id && !role.IsDeleted);

            if (role == null) return NotFound();

            role.Delete();

            return NoContent();
        }
    }
}
