using Microsoft.AspNetCore.Mvc;

namespace Kikolo_v1.Controllers
{
    public class MiniGamesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Reaktion()
        {
            return View();
        }
    }
}
