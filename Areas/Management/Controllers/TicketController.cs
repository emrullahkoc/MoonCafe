using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonCafe.Models;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class TicketController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        [HttpGet]
        public IActionResult Index()
        {
            var model = db.Tickets
                 .Include(x => x.Activity)
                 .ThenInclude(x => x.Artist)
                 .Include(x => x.Activity)
                 .ThenInclude(x => x.Category)
                 .Include(x => x.Activity)
                 .Include(x => x.User)
                 .OrderBy(x => x.CreateDate)
                 .ToList();
            return View(model);
        }
        public async Task<JsonResult> DeactivateByJs(Guid Id)
        {
            var ticket = await db.Tickets.FindAsync(Id);
            if (ticket == null)
            {
                return Json("No Such Ticket Found");
            }
            ticket.TicketStatus = false;
            ticket.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json("Ticket Successfully Deactivate");
        }

        public async Task<JsonResult> ActivateByJs(Guid Id)
        {
            var ticket = await db.Tickets.FindAsync(Id);
            if (ticket == null)
            {
                return Json("No Such Ticket Found");
            }
            ticket.TicketStatus = true;
            ticket.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Json("Ticket Successfully Activate");
        }
    }
}
