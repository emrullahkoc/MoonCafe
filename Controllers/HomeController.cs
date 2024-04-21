using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using MoonCafe.Models;
using MoonCafe.Models.ViewModels;
using MoonCafe.Utils;
using System;
using System.Diagnostics;

namespace MoonCafe.Controllers
{
    public class HomeController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        private readonly IWebHostEnvironment _hostEnvironment;
        public HomeController(IWebHostEnvironment hostEnviroment)
        {
            _hostEnvironment = hostEnviroment;
        }

        [HttpGet]
        public IActionResult Contact()
        {
            ViewBag.Address = db.Abouts.FirstOrDefault()?.Address;
            ViewBag.Telephone = db.Abouts.FirstOrDefault()?.Telephone;
            ViewBag.Mail = db.Abouts.FirstOrDefault()?.Mail;
            ViewBag.AboutMapLocation = db.Abouts.FirstOrDefault()?.AboutMapLocation;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(Contact model, IFormFile? img)
        {
            if (img != null)
            {
                model.ContactImageUrl = await ImageUploader.UploadImageAsync(_hostEnvironment, img);
            }
            else
            {
                model.ContactImageUrl = "/ImageUpload/logo2.png";
            }
            model.ContactFullName = model.ContactFullName.ToUpper();
            model.ContactStatus = false;
            model.ContactMail = model.ContactMail;
            model.ContactDate = DateTime.Now;
            model.ContactMessage = model.ContactMessage;
            db.Contacts.Add(model);
            db.SaveChanges();

            MemoryStream imageStream = null;
            if (img != null)
            {
                imageStream = new MemoryStream();
                await img.CopyToAsync(imageStream);
                imageStream.Position = 0;
                MailSender.SendAdmin("Comment Written", $"{model.ContactFullName} </br></br> {model.ContactMessage} </br></br> {model.ContactMail} </br></br> {model.ContactDate}", imageStream, img != null ? img.FileName : null);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new HomeViewModel();
            model.Title = db.MainPages.FirstOrDefault()?.Title;
            model.TitleT = db.MainPages.FirstOrDefault()?.TitleT;
            model.MainPageDescription = db.MainPages.FirstOrDefault()?.MainPageDescription;

            model.Activities = db.Activities
                                    .Where(x => x.ActivityStatus == true)
                                    .Include(x => x.Artist)
                                    .Include(x => x.Category)
                                    .Where(x => x.ActivityDate >= DateTime.Now)
                                    .OrderBy(x => x.ActivityDate)
                                    .Take(10).ToList();

            model.Artists = db.Artists
                                    .Where(x => x.ArtistStatus == true)
                                    .OrderByDescending(x => x.UpdateDate)
                                    .Take(4).ToList();

            model.FinishedActivities = db.Activities
                                    .Include(x => x.Artist)
                                    .Include(x => x.Category)
                                    .Where(x => x.ActivityDate <= DateTime.Now)
                                    .OrderByDescending(x => x.ActivityDate)
                                    .Take(3).ToList();

            model.Contacts = db.Contacts
                                    .Where(x => x.ContactStatus == true)
                                    .OrderByDescending(x => x.ContactDate)
                                    .Take(10).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult About()
        {
            About model = db.Abouts.FirstOrDefault();
            return View(model);
        }

        [HttpGet]
        public IActionResult Artists()
        {
            var model = db.Artists.Where(x => x.ArtistStatus == true)
                .OrderByDescending(x => x.CreateDate)
                .Include(x => x.Activities)
                .ThenInclude(x => x.Category)
                .ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult ArtistsDetails(int id)
        {
            var model = db.Artists.Where(x => x.ArtistStatus == true)
                .Include(x => x.Activities.Where(x => x.ActivityDate >= DateTime.Now))
                .ThenInclude(x => x.Category)
                .FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                return Redirect("/Home/Artists");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Activities()
        {
            var model = db.Activities.Where(x => x.ActivityStatus == true)
                .OrderByDescending(x => x.CreateDate)
                .Include(x => x.Category)
                .Include(x => x.Artist)
                .Where(x => x.ActivityDate >= DateTime.Now)
                .OrderBy(x => x.ActivityDate)
                .Take(10).ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult ActivitiesDetails(int id)
        {
            var model = db.Activities
                .Include(x => x.Category)
                .Include(x => x.Artist)
                .FirstOrDefault(x => x.Id == id);
            return View(model);
        }
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
