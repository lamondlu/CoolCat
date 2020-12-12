using Microsoft.AspNetCore.Mvc;

namespace CoolCat.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
