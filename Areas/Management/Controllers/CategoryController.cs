using MoonCafe.Models;
using MoonCafe.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonCafe.Models.ViewModels;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        private readonly IWebHostEnvironment _hostEnvironment;
        public CategoryController(IWebHostEnvironment hostEnviroment)
        {
            _hostEnvironment = hostEnviroment;
        }

        [HttpGet]
        public IActionResult Index(string CategoryName)
        {
            var model = db.Categories
                .ToList();
            var model2 = new DashboardViewModel();
            model2.ActivityCount = db.Activities.Include(x => x.Category).Where(x => x.ActivityDate >= DateTime.Now).Count(x => x.ActivityStatus == true);

            if (!string.IsNullOrEmpty(CategoryName))
            {
                ViewBag.Search = CategoryName;
                model = model
                    .Where(c => c.CategoryName.ToLower()
                    .Contains(CategoryName.ToLower()))
                    .ToList();
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    model.CategoryImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
                }
                else
                {
                    model.CategoryImageUrl = "/ImageUpload/logo2.png";
                }
                model.CategoryName = model.CategoryName.ToUpper();
                model.CategoryDescription = model.CategoryDescription;
                model.CategoryStatus = true;
                db.Categories.Add(model);
                db.SaveChanges();
                return Redirect("/Management/Category/Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Category? model = db.Categories.Find(id);
            if (model == null)
            {
                return Redirect("/Management/Category/Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                Category? editmodel = db.Categories.Find(model.Id);
                if (editmodel == null)
                {
                    return Redirect("/Management/Category/Index");
                }
                if (img != null)
                {
                    await ImageUploader.DeleteImageAsync(_hostEnvironment, editmodel.CategoryImageUrl);
                    editmodel.CategoryImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
                }
                editmodel.CategoryName = model.CategoryName.ToUpper(); ;
                editmodel.CategoryDescription = model.CategoryDescription;
                editmodel.CategoryStatus = true;
                await db.SaveChangesAsync();
                return Redirect("/Management/Category/Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Category? model = db.Categories.Include(a => a.Activities
            .Where(a => a.ActivityStatus == true))
                .ThenInclude(b => b.Artist)
                .FirstOrDefault(a => a.Id == id);
            if (model == null)
            {
                return Redirect("/Management/Category/Index");
            }
            return View(model);
        }

        public async Task<JsonResult> DeactivateByJs(int Id)
        {
            var category = await db.Categories.FindAsync(Id);
            if (category == null)
            {
                return Json("No Such Category Found");
            }
            category.CategoryStatus = false;
            await db.SaveChangesAsync();
            return Json("Category Successfully Deactivate");
        }

        public async Task<JsonResult> ActivateByJs(int Id)
        {
            var category = await db.Categories.FindAsync(Id);
            if (category == null)
            {
                return Json("No Such Category Found");
            }
            category.CategoryStatus = true;
            await db.SaveChangesAsync();
            return Json("Category Successfully Activate");
        }
    }
}
