using Microsoft.AspNetCore.Mvc;

namespace Mystique.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
