using Microsoft.AspNetCore.Mvc;

namespace Sada.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TituloController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("listarTitulos")]
        public IActionResult Listar()
        {
            return Ok();
        }
    }
}
