using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonCafe.Models;
using QRCoder;
using System.Security.Claims;

namespace MoonCafe.Controllers
{
    public class TicketController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();
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
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                model.ActivityId= Convert.ToInt32(id);
                model.UserId = Convert.ToInt32(a);
                model.NumberPeople = NumberPeople;
                model.CreateDate = DateTime.Now;
                model.TicketStatus = false;
                db.Tickets.Add(model);
                db.SaveChanges();
                return Redirect("~/Ticket/Index");
            }
            return View(model);
        }

        [HttpGet("qrcode/{id}")]
        public IActionResult GenerateQRCode(Guid id)
        {
            Ticket? model = db.Tickets.Find(id);
            Ticket myTicket = GetMyTicketById(id);

            if (myTicket == null)
            {
                return NotFound();
            }
            var qr = new
            {
                Name = myTicket.User.UserFullName,
                Activity = myTicket.Activity.ActivityName,
                Ticket = model.Id,
            };
            QRCodeGenerator generator = new();
            QRCodeData data = generator.CreateQrCode(qr.ToString(), QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new(data);
            byte[] byteGraphic = qrCode.GetGraphic(5, new byte[] { 84, 99, 71 }, new byte[] { 240, 246, 240 });
            return File(byteGraphic, "image/png");
        }

        private Ticket GetMyTicketById(Guid id)
        {
            using (var context = new MoonCafeContext())
            {
                return context.Tickets.Include(x => x.User).Include(x => x.Activity).FirstOrDefault(t => t.Id == id);
            }
        }

        public IActionResult MyTicket()
        {
            return View();
        }
    }
}
