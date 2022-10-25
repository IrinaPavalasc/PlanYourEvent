using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProiectLicenta.DataAccess;
using ProiectLicenta.Models;
using System.Security.Claims;

namespace ProiectLicenta.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventController : Controller
    {
        private ApplicationDbContext db;

        public EventController(ApplicationDbContext database)
        {
            db = database;
        }

        public IActionResult Index()
        {

            IEnumerable<Event> EventList = db.Event;
            return View(EventList);

        }

        //Get
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event obj)
        {
            if (ModelState.IsValid)
            {

                db.Event.Add(obj);
                db.SaveChanges();
               
                TempData["success"] = "Event added successfully!";
                return RedirectToAction("Index");

            }
            return View(obj);

        }

        //Get
        public IActionResult Edit(int? eventId)
        {
            if (eventId == null || eventId == 0)
            {
                return NotFound();
            }

            var even = db.Event.Find(eventId);

            return View(even);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Event even)
        {
            if (ModelState.IsValid)
            {
                TempData["success"] = "Event details updated successfully!";

                db.Event.Update(even);
                db.SaveChanges();

                return RedirectToAction("Index");

            }

            return View(even);

        }
        //Get
        public IActionResult Delete(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }
            var even = db.Event.Find(eventId);
            return View(even);
        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Event even)
        {

            db.Event.Remove(even);
            db.SaveChanges();
 
            TempData["success"] = "Event deleted successfully!";
            return RedirectToAction("Index");


        }
    }
}
