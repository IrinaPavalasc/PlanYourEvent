using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProiectLicenta.DataAccess;
using ProiectLicenta.Models;
using ProiectLicenta.Models.ViewModels;
using System.Security.Claims;


namespace ProiectLicenta.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationDbContext db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserController(ApplicationDbContext database, IWebHostEnvironment hostEnvironment)
        {
            db = database;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index(string? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            
            var user = db.User.Where(i => i.Id == Id).FirstOrDefault();
            if(user.Role == "Provider")
            {
                return View(user);
            }

            return RedirectToAction("Index", "Home", new { area = "Client" });

        }


        //Get
        public IActionResult Edit(string? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var user = db.User.Find(Id);

            return View(user);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                var obj = db.User.FirstOrDefault(i => i.Id == user.Id);
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\profiles");
                    var extension = Path.GetExtension(file.FileName);

                    if (user.ImageUrl != null)
                    {
                        var oldImgPath = Path.Combine(wwwRootPath, user.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.Diagnostics.Debug.WriteLine("oldImgPathDelete");
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    if (obj != null) {
                        obj.ImageUrl = @"\images\profiles\" + fileName + extension;
                    }
                        
                }

                if (obj != null)
                {
                    obj.Name = user.Name;
                    obj.PhoneNumber = user.PhoneNumber;
                    obj.Country = user.Country;
                }
                db.User.Update(obj);
                db.SaveChanges();
                TempData["success"] = "User details updated successfully!";
                return RedirectToAction("Index", "User", new { user.Id });

            }
            return View(user);

        }
        //Get
        public IActionResult Delete(string? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var user = db.User.Find(Id);


            return View(user);
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

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(User user)
        {

            if (user.ImageUrl != "\\images\\noimage\\NoImageAvailable.jpg")
            {
                var oldImgPath = Path.Combine(_hostEnvironment.WebRootPath, user.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }
            }
            var venueList = db.Venue.Where(i => i.ProviderId == user.Id).ToList();
            foreach (var venue in venueList)
            {
                var address = db.Address.Where(i=>i.AddressId == venue.AddressId).SingleOrDefault();
                db.Address.Remove(address);
                db.Venue.Remove(venue);
                DeleteVenueCalendar(venue.VenueId);

            }

            db.User.Remove(user);
            db.SaveChanges();
            TempData["success"] = "User deleted successfully!";
            return RedirectToAction("Index", "Home", new { area = "Client" });

        }
    }
}
