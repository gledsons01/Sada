using Microsoft.AspNetCore.Mvc;

namespace Sada.Web.Controllers;

public class UsuarioController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
