using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.DataAccess;
using ProiectLicenta.Models;
using System.Security.Claims;

namespace ProiectLicenta.Areas.Provider.Controllers
{
    [Area("Provider")]
    [Authorize(Roles = "Provider")]
    public class CalendarController : Controller
    {
        private ApplicationDbContext db;

        public CalendarController(ApplicationDbContext database)
        {
            db = database;
        }

        //GET
        public IActionResult Index(int? venueId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ViewBag.currentUserId = currentUserId.Value;

            if (venueId == null)
            {
                return NotFound();
            }
            IEnumerable<Calendar> CalendarList = db.Calendar.Where(i => i.VenueId == venueId);
            var venue = db.Venue.Find(venueId);
            ViewBag.VenueName = venue.Name;
            return View(CalendarList);
        }

        //POST
        [HttpPost]
        public IActionResult Edit(int price, bool availability, string start, string end, int venueId)
        {

            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            for(DateTime date = startDate; date.Date < endDate.Date; date = date.AddDays(1))
            {
                var stringDate = date.ToString("yyyy-MM-dd");
                var calendarDay = db.Calendar.Where(i => i.Date == stringDate && i.VenueId == venueId).SingleOrDefault();
                calendarDay.Available = availability;
                calendarDay.PricePerDay = price;
                db.Calendar.Update(calendarDay);
                db.SaveChanges();
            }

            return RedirectToAction("Index", new {venueId = venueId});

        }



    }
}
