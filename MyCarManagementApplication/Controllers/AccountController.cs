using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCarManagementApplication.Data;
using MyCarManagementApplication.Models;
using System.Threading.Tasks;

namespace MyCarManagementApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly CarDbContext _context;

        public AccountController(CarDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Welcome", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Username and password are required.";
                return View();
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Username is already taken.";
                return View();
            }

            var user = new User
            {
                Username = username,
                Password = password 
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful. Please log in.";
            return RedirectToAction("Login");
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsLoggedIn");
            return RedirectToAction("Login");
        }
    }
}
