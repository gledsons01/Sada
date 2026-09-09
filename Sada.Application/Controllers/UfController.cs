using Microsoft.AspNetCore.Mvc;

namespace Sada.Application.Controllers
{
    public class UfController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
