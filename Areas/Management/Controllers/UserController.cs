using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MoonCafe.Models;
using MoonCafe.Utils;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        [HttpGet]
        public IActionResult Index(string UserFullName)
        {
            ViewBag.Search = "";
            var model = db.Users
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
            if (!string.IsNullOrEmpty(UserFullName))
            {
                ViewBag.Search = UserFullName;
                model = model
                    .Where(c => c.UserFullName.ToLower()
                    .Contains(UserFullName))
                    .ToList();
            }
            return View(model);
        }
        public async Task<JsonResult> DeactivateByJs(int Id)
        {
            var user = await db.Users.FindAsync(Id);
            if (user == null)
            {
                return Json("No Such User Found");
            }
            user.UserStatus = false;
            user.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json("User Successfully Deactivate");
        }

        public async Task<JsonResult> ActivateByJs(int Id)
        {
            var user = await db.Users.FindAsync(Id);
            if (user == null)
            {
                return Json("No Such User Found");
            }
            user.UserStatus = true;
            user.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json("User Successfully Activate");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            User? model = db.Users
                .FirstOrDefault(a => a.Id == id);
            if (model == null)
            {
                return Redirect("/Management/User/Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            User? model = db.Users.Find(id);
            if (model == null)
            {
                return Redirect("/Management/User/Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                User? editmodel = db.Users.Find(model.Id);
                if (editmodel == null)
                {
                    return Redirect("/Management/User/Index");
                }
                editmodel.UserFullName = model.UserFullName; ;
                editmodel.Role = model.Role;
                editmodel.UserEmail = model.UserEmail;
                editmodel.UserPassword = model.UserPassword;
                editmodel.CreatedDate = model.CreatedDate;
                editmodel.UpdateDate = DateTime.Now;
                editmodel.UserStatus = true;
                await db.SaveChangesAsync();
                return Redirect("/Management/User/Index");
            }
            return View(model);
        }
    }
}
