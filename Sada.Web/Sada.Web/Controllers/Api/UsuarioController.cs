using Microsoft.AspNetCore.Mvc;

namespace Sada.Web.Controllers.Api
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
