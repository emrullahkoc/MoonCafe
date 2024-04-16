using Microsoft.AspNetCore.Mvc;

namespace MoonCafe.Controllers
{
    public class ErrorPageController : Controller
    {
        public IActionResult Error1()
        {
            return View();
        }
    }
}
