using LikesApi.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LikesApi.Controllers
{
    [Route("api/like")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ValuesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public IActionResult Get()
        {


            return Ok("Hello world");
        }
    }
}
