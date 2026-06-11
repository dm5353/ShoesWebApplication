using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesWebApplication.Models;
using System.Security.Claims;


namespace ShoesWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;

        private readonly PasswordHasher<User> _passwordHasher = new();

        public AccountController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _context.Users
                .Include(e => e.UserRole)
                .FirstOrDefaultAsync(e => e.Login == login);

            /*user.PasswordHash = _passwordHasher.HashPassword(user, password);
            await _context.SaveChangesAsync();*/

            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    password);

                if (result == PasswordVerificationResult.Success)
                {

                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.UserRole.RoleName)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Goods");
                }
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuestLogin()
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "Гость"),
        new Claim("UserId", "0"),
        new Claim(ClaimTypes.Role, "Гость")
    };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Goods");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
