using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonCafe.Models;
using MoonCafe.Utils;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class AboutController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        private readonly IWebHostEnvironment _hostEnvironment;
        public AboutController(IWebHostEnvironment hostEnviroment)
        {
            _hostEnvironment = hostEnviroment;
        }

        [HttpGet]
        public IActionResult Edit()
        {
            About? model = db.Abouts.FirstOrDefault();
            if (model == null)
            {
                return Redirect("/Management/Home/Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(About model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                About? editModel = db.Abouts.FirstOrDefault();
                if (editModel == null)
                {
                    return Redirect("/Management/Home/Index");
                }
                if (img != null)
                {
                    await ImageUploader.DeleteImageAsync(_hostEnvironment, editModel.AboutImageUrl);
                    editModel.AboutImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
                }
                editModel.AboutTitle = model.AboutTitle.ToUpper();
                editModel.AboutDetails = model.AboutDetails;
                editModel.Address = model.Address;
                editModel.Mail = model.Mail;
                editModel.Telephone = model.Telephone;
                editModel.AboutMapLocation = model.AboutMapLocation;
                db.SaveChanges();
                return Redirect("/Management/Home/Index");
            }
            return View(model);
        }
    }
}
