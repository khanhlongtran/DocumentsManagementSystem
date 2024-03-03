using LoginLogoutExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LoginLogoutExample.Controllers
{
    public class AccountController : Controller
    {
        private readonly LoginLogoutExampleContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(LoginLogoutExampleContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [Route("/login")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {

            // Lỗi thì cho về trang đăng nhập, chuẩn thì cho sang Home Index
            if (ModelState.IsValid)
            {
                var email = model.Email.Trim();
                var password = model.Password.Trim();
                var userDetails = _context.Userdetails.SingleOrDefault(u => u.Email.Equals(email) && u.Password.Equals(password));
                if (userDetails == null)
                {
                    return View("Index");
                }
                else
                {
                    HttpContext.Session.SetString("userId", userDetails.Name);
                }
            }
            else
            {
                return View("Index");
            }
            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        public async Task<ActionResult> Register(RegistrationViewModel model)
        {

            if (ModelState.IsValid)
            {
                Userdetail user = new Userdetail
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Mobile = model.Mobile

                };
                _context.Userdetails.Add(user);
                await _context.SaveChangesAsync();

            }
            else
            {
                // Cho về lại trang đăng kí
                return View("Registration");
            }
            return RedirectToAction("Index", "Account");
        }
        [Route("/register")]
        public IActionResult Registration()
        {
            ViewData["Message"] = "Registration Page";

            return View();
        }
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

    }
}
