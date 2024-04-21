using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonCafe.Models;
using MoonCafe.Utils;
using System.Security.Claims;

namespace MoonCafe.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        [HttpGet]
        public IActionResult QrCod(Guid id)
        {
            if (id != null)
            {
                var qr = ZXingQrCode.GenerateQR(id);
                return Content(qr, "image/svg+xml");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            Activity? model = db.Activities
                .Include(x => x.Artist)
                .FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                return Redirect("/Ticket/Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Ticket model, int id, int NumberPeople)
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var b = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            var activityId = db.Activities.Find(id);

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                model.ActivityId = Convert.ToInt32(id);
                model.UserId = Convert.ToInt32(a);
                model.NumberPeople = NumberPeople;
                model.CreateDate = DateTime.Now;
                model.TicketStatus = false;
                db.Tickets.Add(model);
                db.SaveChanges();

                MailSender.SendAdmin("Ticket Created", $"{b} </br></br> {activityId.ActivityName} </br></br>{activityId.ActivityDate} </br></br> {model.NumberPeople} NUMBER PEOPLE </br></br> AMOUNT TO BE PAID {model.NumberPeople * activityId.ActivityPrice} </br></br> {model.CreateDate}", null, null);

                return Redirect("~/Home/Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult MyTicket()
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            int userId;
            if (!int.TryParse(a, out userId))
            {
                return BadRequest("Invalid user ID");
            }

            var model = db.Tickets
                .Include(x => x.Activity)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Activity)
                .Include(x => x.User).Where(x => x.UserId == userId)
                .OrderBy(x => x.Activity.ActivityDate)
                .ToList();

            if (model.Any(x => x.Activity.ActivityDate < DateTime.Now))
            {
                var activitiesWithPastDates = model.Where(x => x.Activity.ActivityDate < DateTime.Now);
                activitiesWithPastDates.ToList().ForEach(x => x.TicketStatus = false);
                db.SaveChanges();
            }
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
            return Json("Your ticket has been successfully canceled");
        }
    }
}
