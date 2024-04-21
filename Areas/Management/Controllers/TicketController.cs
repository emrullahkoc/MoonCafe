using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoonCafe.Models;
using System.Drawing.Imaging;
using System.Drawing;
using System.Net.NetworkInformation;
using ZXing;
using ZXing.Common;
using MoonCafe.Models.ViewModels;

namespace MoonCafe.Areas.Management.Controllers
{
    [Area("Management")]
    [Authorize(Roles = "Admin")]
    public class TicketController : Controller
    {
        MoonCafeContext db = new MoonCafeContext();

        private readonly ILogger<TicketController> _logger;

        public TicketController(ILogger<TicketController> logger)
        {
            _logger = logger;
        }

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
                 .OrderByDescending(x => x.Activity.ActivityDate)
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
            ticket.ControlDateTime = null;
            ticket.UpdateDate = null;
            await db.SaveChangesAsync();
            return Json("Ticket Successfully Activate");
        }
        [HttpGet]
        public IActionResult TicketControl()
        {
            var model = new DashboardViewModel();
            model.Activities = db.Activities
                                    .Where(x => x.ActivityStatus == true)
                                    .Include(x => x.Artist)
                                    .Include(x => x.Category)
                                    .Include(x => x.Tickets.Where(x => x.TicketStatus == true))
                                    .Where(x => x.ActivityDate >= DateTime.Now && x.ActivityDate <= DateTime.Now.AddHours(24))
                                    .OrderBy(x => x.ActivityDate)
                                    .ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Camera(int id)
        {
            Activity? model = db.Activities.Find(id);
            if (model == null)
            {
                return Redirect("/Management/Ticket/TicketControl");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetData(string data, int id)
        {
            try
            {
                var base64Data = data.Split(',')[1];
                var imageBytes = Convert.FromBase64String(base64Data);
                using (var memoryStream = new MemoryStream(imageBytes))
                {
                    Image image = Image.FromStream(memoryStream);
                    Bitmap bmp = new Bitmap(image);
                    byte[] rgbBytes = ConvertBitmapToRgbBytes(bmp);
                    RGBLuminanceSource source = new RGBLuminanceSource(rgbBytes, bmp.Width, bmp.Height);
                    BinaryBitmap binaryBitmap = new BinaryBitmap(new HybridBinarizer(source));
                    var reader = new ZXing.MultiFormatReader();
                    var result = reader.decode(binaryBitmap);
                    if (result != null)
                    {
                        string qrData = result.Text;
                        Guid ticketId;
                        if (Guid.TryParse(qrData, out ticketId))
                        {
                            var info = db.Tickets.Include(x => x.User).Include(x => x.Activity).ThenInclude(x => x.Category).FirstOrDefault(x => x.Id == ticketId);
                            var ticket = db.Tickets.FirstOrDefault(t => t.Id == ticketId);

                            if (info.ActivityId == id)
                            {
                                if (ticket != null && ticket.TicketStatus == true)
                                {
                                    if (ticket.ControlDateTime == null)
                                    {
                                        ticket.ControlDateTime = DateTime.Now;
                                        await db.SaveChangesAsync();
                                        return Json(new { success = true, message = $"{info.User.UserFullName} {info.NumberPeople} PEOPLE LOGIN DONE" });
                                    }
                                    else if (ticket.UpdateDate == null && ticket.ControlDateTime != null && ticket.ControlDateTime <= DateTime.Now.AddMinutes(-30))
                                    {
                                        ticket.UpdateDate = DateTime.Now;
                                        await db.SaveChangesAsync();
                                        return Json(new { success = true, message = $"{info.User.UserFullName} EXIT DONE" });
                                    }
                                    else if (ticket.UpdateDate == null && ticket.ControlDateTime != null && ticket.ControlDateTime > DateTime.Now.AddMinutes(-30))
                                    {
                                        return Json(new { success = true, message = $"{info.User.UserFullName} YOU CANNOT LEAVE THE CAFE FOR THE FIRST HALF HOUR" });
                                    }
                                    else if (ticket.UpdateDate != null && ticket.ControlDateTime != null && ticket.UpdateDate > DateTime.Now.AddMinutes(-60))
                                    {
                                        ticket.ControlDateTime = DateTime.Now;
                                        ticket.UpdateDate = null;
                                        ticket.TicketStatus = false;
                                        await db.SaveChangesAsync();
                                        return Json(new { success = true, message = $"{info.User.UserFullName} LOGIN DONE" });
                                    }
                                    else if (ticket.UpdateDate != null && ticket.ControlDateTime != null && ticket.UpdateDate <= DateTime.Now.AddMinutes(-60))
                                    {
                                        return Json(new { success = true, message = $"{info.User.UserFullName} YOU CAN ONLY LOG IN DURING THE FIRST HOUR" });
                                    }
                                }
                                else if (ticket.TicketStatus == false)
                                {
                                    if (ticket.ControlDateTime != null)
                                    {
                                        ticket.ControlDateTime = null;
                                        ticket.UpdateDate = DateTime.Now;
                                        await db.SaveChangesAsync();
                                        return Json(new { success = true, message = $"{info.User.UserFullName} EXIT DONE" });
                                    }
                                    else if (ticket.UpdateDate != null)
                                    {
                                        return Json(new { success = true, message = $"{info.User.UserFullName} PLEASE MAKE PAYMENT TO LOG IN AGAIN" });
                                    }
                                    else if (true)
                                    {
                                        return Json(new { success = true, message = $"{info.User.UserFullName} PLEASE MAKE PAYMENT" });
                                    }
                                }
                            }
                            else
                            {
                                return Json(new { success = true, message = $"{info.User.UserFullName} YOU DO NOT HAVE A TICKET FOR THIS EVENT" });
                            }
                        }
                        else
                        {
                            return Json(new { success = true, message = "QR CODE NOT FOUND OR UNREADABLE" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "QR CODE NOT FOUND OR UNREADABLE" });
                    }
                    return Json(new { success = true, message = "QR CODE NOT FOUND OR UNREADABLE" });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        public byte[] ConvertBitmapToRgbBytes(Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int bytesCount = bmpData.Stride * bmp.Height;
            byte[] rgbBytes = new byte[bytesCount];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rgbBytes, 0, bytesCount);
            bmp.UnlockBits(bmpData);
            return rgbBytes;
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            Ticket? model = db.Tickets
                .Include(x=> x.User)
                .Include(x=> x.Activity)
                .ThenInclude(x => x.Category)
                .Include(x => x.Activity)
                .ThenInclude(x => x.Artist)
                .FirstOrDefault(a => a.Id == id);
            if (model == null)
            {
                return Redirect("/Management/Ticket/Index");
            }
            return View(model);
        }
    }
}
