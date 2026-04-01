using Microsoft.AspNetCore.Mvc;

namespace digiEasy_backend.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new
            {
                message = "digiEasy API",
                swagger = "/swagger"
            });
        }
    }
}
