using Microsoft.AspNetCore.Mvc;

namespace Sada.Web.Controllers.Api
{
    public class UfController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
