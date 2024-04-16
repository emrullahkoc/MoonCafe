using MoonCafe.Models;
using MoonCafe.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class ArtistController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        private readonly IWebHostEnvironment _hostEnvironment;
        public ArtistController(IWebHostEnvironment hostEnviroment)
        {
            _hostEnvironment = hostEnviroment;
        }

        [HttpGet]
        public IActionResult Index(string ArtistName)
        {
            //var a = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            ViewBag.Search = "";
            var model = db.Artists
                .OrderByDescending(x => x.CreateDate)
                .ToList();
            if (!string.IsNullOrEmpty(ArtistName))
            {
                ViewBag.Search = ArtistName;
                model = model
                    .Where(c => c.ArtistName
                    .Contains(ArtistName))
                    .ToList();
            }
            return View(model);
        }

        public async Task<JsonResult> DeactivateByJs(int Id)
        {
            var artist = await db.Artists.FindAsync(Id);
            if (artist == null)
            {
                return Json("No Such Artist Found");
            }
            artist.ArtistStatus = false;
            artist.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json("Artist Successfully Deactivate");
        }

        public async Task<JsonResult> ActivateByJs(int Id)
        {
            var artist = await db.Artists.FindAsync(Id);
            if (artist == null)
            {
                return Json("No Such Artist Found");
            }
            artist.ArtistStatus = true;
            artist.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json("Artist Successfully Activate");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Artist model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    model.ArtistImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
                }
                else
                {
                    model.ArtistImageUrl = "/ImageUpload/logo2.png";
                }
                model.ArtistName = model.ArtistName.ToUpper();
                model.ArtistStatus = true;
                model.ArtistDescription = model.ArtistDescription;
                model.CreateDate = DateTime.Now;
                db.Artists.Add(model);
                db.SaveChanges();
                return Redirect("/Management/Artist/Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Artist? model = db.Artists.Find(id);
            if (model == null)
            {
                return Redirect("/Management/Artist/Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Artist model, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                Artist? editmodel = db.Artists.Find(model.Id);
                if (editmodel == null)
                {
                    return Redirect("/Management/Artist/Index");
                }
                if (img != null)
                {
                    await ImageUploader.DeleteImageAsync(_hostEnvironment, editmodel.ArtistImageUrl);
                    editmodel.ArtistImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
                }
                editmodel.ArtistName = model.ArtistName.ToUpper(); ;
                editmodel.ArtistDescription = model.ArtistDescription;
                editmodel.ArtistBirthDate = model.ArtistBirthDate;
                editmodel.UpdateDate = DateTime.Now;
                editmodel.ArtistStatus = true;
                await db.SaveChangesAsync();
                return Redirect("/Management/Artist/Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Artist? model = db.Artists.Include(a => a.Activities)
                .ThenInclude(b => b.Category)
                .FirstOrDefault(a => a.Id == id);
            if (model == null)
            {
                return Redirect("/Management/Artist/Index");
            }
            return View(model);
        }
    }
}
