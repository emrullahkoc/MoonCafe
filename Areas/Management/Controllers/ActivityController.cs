using MoonCafe.Models;
using MoonCafe.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class ActivityController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        private readonly IWebHostEnvironment _hostEnvironment;
        public ActivityController(IWebHostEnvironment hostEnviroment)
        {
            _hostEnvironment = hostEnviroment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = db.Activities
               .Include(x => x.Category)
               .Include(x => x.Artist)
               .OrderByDescending(x => x.ActivityDate)
               .ToList();

            if (model.Any(x => x.ActivityDate < DateTime.Now))
            {
                var activitiesWithPastDates = model.Where(x => x.ActivityDate < DateTime.Now);
                activitiesWithPastDates.ToList().ForEach(x => x.ActivityStatus = false);
                db.SaveChanges();
            }

            return View(model);
        }

        public async Task<JsonResult> DeactivateByJs(int Id)
        {
            var activity = await db.Activities.FindAsync(Id);
            if (activity == null)
            {
                return Json("No Such Activity Found");
            }
            activity.ActivityStatus = false;
            activity.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json("Activity Successfully Deactivate");
        }

        public async Task<JsonResult> ActivateByJs(int Id)
        {
            var activity = await db.Activities.FindAsync(Id);
            if (activity == null)
            {
                return Json("No Such Activity Found");
            }
            activity.ActivityStatus = true;
            activity.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json("Activity Successfully Activate");
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories.OrderBy(x => x.CategoryName).Where(x => x.CategoryStatus == true), "Id", "CategoryName", null);
            ViewBag.ArtistId = new SelectList(db.Artists.OrderBy(x => x.ArtistName).Where(x => x.ArtistStatus == true), "Id", "ArtistName", null);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Activity model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    model.ActivityImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
                }
                else
                {
                    model.ActivityImageUrl = "/ImageUpload/logo2.png";
                }
                model.ActivityName = model.ActivityName.ToUpper();
                model.ActivityStatus = true;
                model.ActivityDescription = model.ActivityDescription;
                model.ActivityDate = model.ActivityDate;
                model.ActivityPrice = model.ActivityPrice;
                model.CreateDate = DateTime.Now;
                model.UpdateDate = DateTime.Now;
                db.Activities.Add(model);
                db.SaveChanges();
                return Redirect("/Management/Activity/Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories.Where(x => x.CategoryStatus == true), "Id", "CategoryName", model.CategoryId);
            ViewBag.ArtistId = new SelectList(db.Artists.Where(x => x.ArtistStatus == true), "Id", "ArtistName", model.ArtistId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = db.Activities.Find(id);
            if (model == null)
            {
                return Redirect("/Management/Activity/Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories.Where(x => x.CategoryStatus == true), "Id", "CategoryName", model.CategoryId);
            ViewBag.ArtistId = new SelectList(db.Artists.Where(x => x.ArtistStatus == true), "Id", "ArtistName", model.ArtistId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Activity model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                var editModel = db.Activities.Find(model.Id);
                if (editModel == null)
                {
                    return Redirect("/Management/Activity/Index");
                }
                if (img != null)
                {
                    await ImageUploader.DeleteImageAsync(_hostEnvironment, editModel.ActivityImageUrl);
                    editModel.ActivityImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
                }
                editModel.UpdateDate = DateTime.Now;
                editModel.CreateDate = model.CreateDate;
                editModel.ActivityName = model.ActivityName;
                editModel.ActivityDate = model.ActivityDate;
                editModel.ActivityDescription = model.ActivityDescription;
                editModel.CategoryId = model.CategoryId;
                editModel.ArtistId = model.ArtistId;
                editModel.ActivityPrice = model.ActivityPrice;
                db.SaveChanges();
                return Redirect("/Management/Activity/Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories.Where(x => x.CategoryStatus == true), "Id", "CategoryName", model.CategoryId);
            ViewBag.ArtistId = new SelectList(db.Artists.Where(x => x.ArtistStatus == true), "Id", "ArtistName", model.ArtistId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var model = db.Activities.Include("Artist")
                .Include("Category")
                .FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                return Redirect("/Management/Activity/Index");
            }
            return View(model);
        }
    }
}
