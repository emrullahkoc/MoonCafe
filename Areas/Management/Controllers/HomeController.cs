using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonCafe.Models;
using MoonCafe.Models.ViewModels;
using System.Security.Claims;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        public IActionResult Index()
        {
            var model = new DashboardViewModel();
            model.ArtistCount = db.Artists.Count(x => x.ArtistStatus == true);
            model.CategoryCount = db.Categories.Count(x => x.CategoryStatus == true);
            model.ActivityCount = db.Activities.Where(x => x.ActivityDate >= DateTime.Now).Count(x => x.ActivityStatus == true);
            model.Activities = db.Activities
                                    .Where(x => x.ActivityStatus == true)
                                    .Include(x =>x.Artist)
                                    .Include(x =>x.Category)
                                    .Where(x => x.ActivityDate >= DateTime.Now)
                                    .OrderBy(x => x.ActivityDate)
                                    .ToList();
            return View(model);
        }
    }
}