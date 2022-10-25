using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.DataAccess;
using ProiectLicenta.Models;
using ProiectLicenta.Models.ViewModels;
using System.Security.Claims;
using System.Web.WebPages.Html;

namespace ProiectLicenta.Areas.Provider.Controllers
{
    [Area("Provider")]
    [Authorize(Roles = "Provider")]
    public class VenueController : Controller
    {
        private ApplicationDbContext db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public VenueController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            db = database;
            _hostEnvironment = hostEnvironment;
        }

        public void AddCalendar(VenueViewModel obj)
        {
            for (int i = 1; i <= 12; i++)
            {
                for (int j = 1; j <= 31; j++)
                {
                    if (i == 2 && j == 29 || i == 4 && j == 31 || i == 6 && j == 31 || i == 9 && j == 31 || i == 11 && j == 31) { break; }
                    else
                    {
                        var date = new DateTime(@DateTime.Now.Year, i, j);

                        if (DateTime.Compare(@DateTime.Now, date) <= 0)
                        {
                            Calendar calendarObj = new Calendar();

                            calendarObj.PricePerDay = obj.Calendar.PricePerDay;
                            calendarObj.Date = date.ToString("yyyy-MM-dd");
                            calendarObj.Available = true;
                            calendarObj.VenueId = obj.Venue.VenueId;

                            db.Calendar.Add(calendarObj);
                            db.SaveChanges();
                        }
                    }

                }
            }
        }
        public IActionResult Index(string? userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            IEnumerable<Venue> VenueList = db.Venue.Include(i=>i.Event).Where(i => i.ProviderId == userId);
            List<VenueViewModel> List = new List<VenueViewModel>();

            foreach (var venue in VenueList)
            {
                var address = db.Address.Where(i => i.AddressId == venue.AddressId).FirstOrDefault();
                var user = db.User.Where(i => i.Id == venue.ProviderId).FirstOrDefault();
                VenueViewModel obj = new VenueViewModel();
                obj.Venue = venue;
                obj.Address = address;
                obj.User = user;
                List.Add(obj);
            }

            return View(List);


        }
        

        //Get
        public IActionResult Create()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ViewBag.currentUserId = currentUserId.Value;

            var EventList = db.Event.Select(i => new SelectListItem {
                Text = i.Name, Value = i.EventId.ToString() }).ToList();

            var obj = new VenueViewModel()
            {
                Events = EventList
            };

            return View(obj);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VenueViewModel obj, IFormFile? file)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ViewBag.currentUserId = currentUserId.Value;

            
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\venues");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Venue.ImageUrl = @"\images\venues\" + fileName + extension;
                }
                else
                {
                    obj.Venue.ImageUrl = @"\images\noimage\NoImageAvailable.jpg";
                }

                obj.Venue.Event = new List<Event>();
                for (int i = 0; i < obj.Events.Count; i++)
                {

                    if (obj.Events[i].Selected == true)
                    {
                        int id = Int32.Parse(obj.Events[i].Value);
                        var even = db.Event.Find(id);
                        obj.Venue.Event.Add(even);
                    }
                }
                db.Address.Add(obj.Address);
                db.SaveChanges();
                obj.Venue.AddressId = obj.Address.AddressId;
                obj.Venue.ProviderId = currentUserId.Value;

                db.Venue.Add(obj.Venue);
                db.SaveChanges();
                AddCalendar(obj);

                TempData["success"] = "Venue added successfully!";
                return RedirectToAction("Index", new { userId = obj.Venue.ProviderId });

            }
            return View(obj);

        }

        //Get
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var venue = db.Venue.Find(id);
            var address = db.Address.Where(i => i.AddressId == venue.AddressId).FirstOrDefault();
            var user = db.User.Where(i => i.Id == venue.ProviderId).FirstOrDefault();
            if (venue == null || address == null || user == null)
            {
                return NotFound();
            }

  
            var EventList = db.Event.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.EventId.ToString()
            }).ToList();

            VenueViewModel obj = new VenueViewModel()
            {
                Events = EventList
            };
            obj.Venue = venue;
            obj.User = user;
            obj.Address = address;

            return View(obj);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(VenueViewModel obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\venues");
                    var extension = Path.GetExtension(file.FileName);

                    if(obj.Venue.ImageUrl != null)
                    {
                        var oldImgPath = Path.Combine(wwwRootPath, obj.Venue.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Venue.ImageUrl = @"\images\venues\" + fileName + extension;
                }
                /*
                for (int i = 0; i < obj.Events.Count; i++)
                {

                    if (obj.Events[i].Selected == true)
                    {
  
                        int id = Int32.Parse(obj.Events[i].Value);
                        var even = db.Event.Find(id);
                        bool contains = obj.Venue.Event.Any(i => i == even);
                        if(contains == false)
                        {
                            list.Add(even);
                        }
 
                    }
                }*/
                
                db.Address.Update(obj.Address);
                db.SaveChanges();

                obj.Venue.AddressId = obj.Address.AddressId;
                db.Venue.Update(obj.Venue);
                db.SaveChanges();
                
                TempData["success"] = "Venue details updated successfully!";
                return RedirectToAction("Index", new { userId = obj.Venue.ProviderId});


            }

            return View(obj);



        }
        public void DeleteVenueCalendar(int id)
        {
            List<Calendar> calendars = new List<Calendar>();
            calendars = db.Calendar.Where(i => i.VenueId == id).ToList();
            foreach (var cal in calendars)
            {
                db.Calendar.Remove(cal);
            }

        }
        //Get
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var venue = db.Venue.Find(id);
            var address = db.Address.Where(i => i.AddressId == venue.AddressId).FirstOrDefault();
            var calendar = db.Calendar.Where(i => i.VenueId == venue.VenueId).FirstOrDefault();
            var user = db.User.Where(i => i.Id == venue.ProviderId).FirstOrDefault();

            if (venue == null || address == null || user == null)
            {
                return NotFound();
            }
            VenueViewModel obj = new VenueViewModel();
            obj.Venue = venue;
            obj.Address = address;
            obj.Calendar = calendar;
            obj.User = user;
            return View(obj);
        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(VenueViewModel obj)
        {

            if (obj.Venue.ImageUrl != "\\images\\noimage\\NoImageAvailable.jpg")
            {
                var oldImgPath = Path.Combine(_hostEnvironment.WebRootPath, obj.Venue.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }
            }


            var address = db.Address.Find(obj.Venue.AddressId);
            db.Address.Remove(address);
            var venue = db.Venue.Find(obj.Venue.VenueId);
            db.Venue.Remove(venue);
            DeleteVenueCalendar(obj.Venue.VenueId);
            db.SaveChanges();
            TempData["success"] = "Venue deleted successfully!";
            return RedirectToAction("Index", new { userId = obj.Venue.ProviderId });

        }
    }
}
