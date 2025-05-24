using Microsoft.AspNetCore.Mvc;

namespace MyCarManagementApplication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Welcome()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}
