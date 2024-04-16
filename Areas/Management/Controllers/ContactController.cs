using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonCafe.Models;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        [HttpGet]
        public IActionResult Index()
        {
            var model = db.Contacts.OrderByDescending(x => x.ContactDate).ToList();
            return View(model);
        }
        public async Task<JsonResult> DeactivateByJs(int Id)
        {
            var contact = await db.Contacts.FindAsync(Id);
            if (contact == null)
            {
                return Json("No Such Contact Found");
            }
            contact.ContactStatus = false;
            await db.SaveChangesAsync();
            return Json("Contact Successfully Deactivate");
        }

        public async Task<JsonResult> ActivateByJs(int Id)
        {
            var contact = await db.Contacts.FindAsync(Id);
            if (contact == null)
            {
                return Json("No Such Contact Found");
            }
            contact.ContactStatus = true;
            await db.SaveChangesAsync();
            return Json("Contact Successfully Activate");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Contact? model = db.Contacts.FirstOrDefault(a => a.Id == id);
            if (model == null)
            {
                return Redirect("/Management/Contact/Index");
            }
            return View(model);
        }

    }
}
