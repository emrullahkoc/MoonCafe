using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoonCafe.Models;

namespace MoonCafe.Controllers
{
    public class AccountController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(User model)
        {

            var user = db.Users.FirstOrDefault(
            x => x.UserEmail == model.UserEmail
            && x.UserPassword == model.UserPassword
            && x.UserStatus == true);

            if (user != null)
            {
                var claims = new List<Claim>{
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Name, user.UserFullName),
                    new Claim(ClaimTypes.Actor, user.UserImageUrl ?? ""),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())};

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                };

                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Management", controller = "Activity", action = "Index" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { controller = "Home", action = "Index" });
                }

            }
            ViewBag.Message = "Please check your information";
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            var user = new User();
            user.UserImageUrl = "/ImageUpload/logo2.png";
            user.UserFullName = model.UserFullName.ToUpper();
            user.UserEmail = model.UserEmail;
            user.UserPassword = model.UserPassword;
            user.UserStatus = true;
            user.Role = "Customer";
            user.CreatedDate = DateTime.Now;
            db.Users.Add(user);
            int changes = db.SaveChanges();
            if (changes > 0)
            {
                var claims = new List<Claim>{
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Name, user.UserFullName),
                    new Claim(ClaimTypes.Actor, user.UserImageUrl ?? ""),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())};

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                };

                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Management", controller = "Activity", action = "Index" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { controller = "Home", action = "Index" });
                }
            }
            return Redirect("/Home/Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
