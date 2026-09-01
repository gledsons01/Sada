using Microsoft.AspNetCore.Mvc;

namespace Sada.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TituloController : Controller
    {
        public IActionResult Index()
        {
            return View("titulo-lista.componente.html");
        }

        [HttpGet("listarTitulos")]
        public IActionResult Listar()
        {
            return Ok();
        }
    }
}
