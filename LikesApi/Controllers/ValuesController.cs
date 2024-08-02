using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LikesApi.Controllers
{
    [Route("api/like")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello world");
        }
    }
}
