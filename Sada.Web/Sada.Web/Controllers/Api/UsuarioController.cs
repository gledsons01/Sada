using Microsoft.AspNetCore.Mvc;

namespace Sada.Web.Controllers.Api
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View("usuario-lista.component.html");
        }

        [HttpGet("listar-usuarios")]
        public IActionResult ListarUsuarios()
        {
            return Ok("usuario-lista.component.html");
        }
    }
}
