using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.DataAccess;
using ProiectLicenta.Models;
using ProiectLicenta.Models.ViewModels;
using System.Diagnostics;
using System.Web.WebPages.Html;

namespace ProiectLicenta.Areas.Client.Controllers
{
    [Area("Client")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext db;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext database)
        {
            _logger = logger;
            db = database;
        }

        public IActionResult Index(string? Id)
        {
            IEnumerable<Venue> VenueList;
            if (Id == null) {
                VenueList = db.Venue.Include(i => i.Event); 
            }
            else
            {
                VenueList = db.Venue.Include(i => i.Event).Where(i => i.ProviderId == Id);
                ViewBag.UserId = Id;

            }

            List<VenueViewModel> List = new List<VenueViewModel>();

            foreach (var venue in VenueList)
            {
                var address = db.Address.Where(i => i.AddressId == venue.AddressId).FirstOrDefault();
                var user = db.User.Where(i => i.Id == venue.ProviderId).FirstOrDefault();
                ViewBag.UserName = user.Name;
                VenueViewModel obj = new VenueViewModel();
                obj.Venue = venue;
                obj.Address = address;
                obj.User = user;
                List.Add(obj);

            }
            var EventList = db.Event.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.EventId.ToString()
            }).ToList();
            SearchViewModel searchList = new SearchViewModel();
            searchList.VenueList = List;
            searchList.Events = EventList;
            return View(searchList);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SearchViewModel s, bool? Desc)
        {

            IEnumerable<Venue> VenueList = new List<Venue>();

            
            if (s.Search != null && Desc == true)
            {
                
                VenueList = db.Venue.Include(i => i.Event).Where(i => i.Name.Contains(s.Search) || i.Description.Contains(s.Search));

            }
            else if (s.Search != null && Desc == null)
            {
                
                VenueList = db.Venue.Include(i => i.Event).Where(i => i.Name.Contains(s.Search));
            }
            else if (s.Search == null)
            {
     
                VenueList = db.Venue.Include(i => i.Event);
            }

            List<VenueViewModel> List = new List<VenueViewModel>();

            foreach (var venue in VenueList)
            {
                System.Diagnostics.Debug.WriteLine(venue.Name);
                var ok = 1;
                var ok2 = 1;
                var ok3 = 1;
                var ok4 = 1;
                var ok5 = 0;
                var nrSelected = 0;

                if (s.StartDate != null && s.EndDate != null)
                {
                    var startDate = DateTime.Parse(s.StartDate);
                    var endDate = DateTime.Parse(s.EndDate);
                    
                    for (DateTime date = startDate; date.Date < endDate.Date; date = date.AddDays(1))
                    {
                        
                        var stringDate = date.ToString("yyyy-MM-dd");
                        
                        var calendarDay = db.Calendar.Where(i => i.Date == stringDate && i.VenueId == venue.VenueId).SingleOrDefault();
                        if (calendarDay == null)
                        {
                            ok = 0;
                        }
                        else if(calendarDay.Available == false )
                        {
                            ok = 0;
                        }  

                    }
                }
                if (s.MaxCapacity != null)
                {
                    if(venue.Capacity < s.MaxCapacity)
                    {
                        ok2 = 0;
                    }
                }
                var address = db.Address.Where(i => i.AddressId == venue.AddressId).FirstOrDefault();
                if (s.Country != null)
                {
                   
                    if(address.Country != s.Country)
                    {
                        ok3 = 0;
                    }
                }
                if (s.City != null)
                {
            
                    if (address.City != s.City)
                    {
                        ok4 = 0;
                    }
                }
                foreach(var chosenEvent in s.Events)
                {
                    
                    if (chosenEvent.Selected == true)
                    {
                        nrSelected = nrSelected + 1;
                        foreach (var venueEvent in venue.Event)
                        {
                            if (venueEvent.EventId.ToString() == chosenEvent.Value)
                            {
                                ok5 = 1;
                            }
                        }
                    }
                }
                if(nrSelected == 0)
                {
                    ok5 = 1;
                }

                if (ok == 1 && ok2 == 1 && ok3 == 1 && ok4 == 1 && ok5 == 1)
                {
                    
                    var user = db.User.Where(i => i.Id == venue.ProviderId).FirstOrDefault();
                    ViewBag.UserName = user.Name;
                    VenueViewModel obj = new VenueViewModel();
                    obj.Venue = venue;
                    obj.Address = address;
                    obj.User = user;
                    List.Add(obj);
                }
 

            }
           
            SearchViewModel searchList = new SearchViewModel();
            searchList.VenueList = List;
            searchList.MaxCapacity = s.MaxCapacity;
            searchList.Country = s.Country;
            searchList.City = s.City;
            searchList.Search = s.Search;
            searchList.StartDate = s.StartDate;
            searchList.EndDate = s.EndDate;
            searchList.Events = s.Events;
            return View(searchList);

        }
        public IActionResult Details(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }
            VenueViewModel obj = new VenueViewModel();

            var venue = db.Venue.Include(i => i.Event).Where(i => i.VenueId == id).FirstOrDefault();

            IEnumerable<Calendar> CalendarList = db.Calendar.Where(i => i.VenueId == id);


            var address = db.Address.Where(i => i.AddressId == venue.AddressId).FirstOrDefault();
            var user = db.User.Where(i => i.Id == venue.ProviderId).FirstOrDefault();
            obj.Venue = venue;
            obj.Address = address;
            obj.User = user;
            obj.CalendarList = CalendarList;
;
            return View(obj);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}