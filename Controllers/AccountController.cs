using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoonCafe.Models;
using MoonCafe.Models.ViewModels;

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
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(
                x => x.UserEmail == model.UserEmail
                && x.UserPassword == model.UserPassword
                && x.UserStatus == true);

                if (user != null)
                {
                    // Create a list of claims for the user
                    var claims = new List<Claim>{
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Name, user.UserFullName),
                    new Claim(ClaimTypes.Actor, user.UserImageUrl ?? ""),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())};

                    // Create a ClaimsIdentity object for the user
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Create an AuthenticationProperties object to store additional information about the authentication process
                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        // Refreshing the authentication session should be allowed.
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                    };

                    // Sign in the user
                    await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                    // Redirect the user to the appropriate action based on their role
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Home", new { area = "Management", controller = "Activity", action = "Index" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { controller = "Home", action = "Index" });
                    }
                }

                // If the user is not authenticated, display a message indicating that the provided information is incorrect
                ViewBag.Message = "Please check your information";
            }

            // If the model is not valid, display the login page again with the provided model
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
