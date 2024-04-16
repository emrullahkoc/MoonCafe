using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MoonCafe.Models;
using MoonCafe.Utils;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        private readonly IWebHostEnvironment _hostEnvironment;
        public SettingController(IWebHostEnvironment hostEnviroment)
        {
            _hostEnvironment = hostEnviroment;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            User? model = db.Users.Find(id);
            if (model == null)
            {
                return Redirect("/Management/Home/Index");
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
                    return Redirect("/Management/Home/Index");
                }
                if (img != null)
                {
                    await ImageUploader.DeleteImageAsync(_hostEnvironment, editmodel.UserImageUrl);
                    editmodel.UserImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
                }
                editmodel.UserFullName = model.UserFullName; ;
                editmodel.Role = model.Role;
                editmodel.UserEmail = model.UserEmail;
                editmodel.UserPassword = model.UserPassword;
                editmodel.CreatedDate = model.CreatedDate;
                editmodel.UpdateDate = DateTime.Now;
                editmodel.UserStatus = true;
                await db.SaveChangesAsync();
                await HttpContext.SignOutAsync();
                return Redirect("/Management/Home/Index");
            }
            return View(model);
        }
    }
}
