using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using MoonCafe.Models;
using MoonCafe.Utils;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class MainPageController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        [HttpGet]
        public IActionResult Edit()
        {
            MainPage? model = db.MainPages.FirstOrDefault();
            if (model == null)
            {
                return Redirect("/Management/Home/Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MainPage model)
        {
            if (ModelState.IsValid)
            {
                MainPage? editModel = db.MainPages.FirstOrDefault();
                if (editModel == null)
                {
                    return Redirect("/Management/Home/Index");
                }
                editModel.Title = model.Title.ToUpper();
                editModel.TitleT = model.TitleT;
                editModel.MainPageDescription = model.MainPageDescription;
                db.SaveChanges();
                return Redirect("/Management/Home/Index");
            }
            return View(model);
        }
    }
}
