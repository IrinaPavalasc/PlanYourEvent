using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.DataAccess;
using ProiectLicenta.Models;
using ProiectLicenta.Models.ViewModels;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;


namespace ProiectLicenta.Areas.Client.Controllers
{
    [Area("Client")]
    
    public class ReservationController : Controller
    {
        private ApplicationDbContext db;

        public ReservationController(ApplicationDbContext database)
        {
            db = database;
        }
        public int CalculatePrice(string? start, string? end, int? venueId)
        {

            var totalPrice = 0;
            if (start != null && end!=null && venueId != null)
            {
                var startDate = DateTime.Parse(start);
                var endDate = DateTime.Parse(end);

                for (DateTime date = startDate; date.Date < endDate.Date; date = date.AddDays(1))
                {
                    var stringDate = date.ToString("yyyy-MM-dd");
                    var calendarDay = db.Calendar.Where(i => i.Date == stringDate && i.VenueId == venueId).SingleOrDefault();
                    totalPrice = totalPrice + calendarDay.PricePerDay;

                }
            }
            return totalPrice;
        }

        public void UpdateCalendar(string? start, string? end, int? venueId, bool available)
        {


            if (start != null && end != null && venueId != null && available!=null)
            {
                var startDate = DateTime.Parse(start);
                var endDate = DateTime.Parse(end);

                for (DateTime date = startDate; date.Date < endDate.Date; date = date.AddDays(1))
                {
                    var stringDate = date.ToString("yyyy-MM-dd");
                    var calendarDay = db.Calendar.Where(i => i.Date == stringDate && i.VenueId == venueId).SingleOrDefault();
                    calendarDay.Available = available;
                    db.Calendar.Update(calendarDay);
                    db.SaveChanges();

                }
            }

        }
        [Authorize(Roles = "Provider,Client")]
        public IActionResult Index(string? status)
        {
            if (status == null)
            {
                status = "All";
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = db.User.Find(currentUserId.Value);
            IEnumerable<Reservation> ReservationList = new List<Reservation>();

            if (user.Role == "Client")
            {
                if (status == "All")
                {
                    ReservationList = db.Reservation.Where(i => i.ClientId == currentUserId.Value);
                }
                else if(status == "Paid")
                {
                    ReservationList = db.Reservation.Where(i => i.ClientId == currentUserId.Value && i.PaymentStatus == "Approved");
                }
                else if (status == "Cancelled")
                {
                    ReservationList = db.Reservation.Where(i => i.ClientId == currentUserId.Value && i.Status == status);
                }
                else
                {
                    ReservationList = db.Reservation.Where(i => i.ClientId == currentUserId.Value && i.Status == status && i.PaymentStatus == "Pending");
                }
            }
            else if (user.Role == "Provider")
            {
                if (status == "All")
                {
                    ReservationList = db.Reservation.Include(i => i.Venue).Where(i => i.Venue.ProviderId == currentUserId.Value);
                }
                else if (status == "Paid")
                {
                    ReservationList = db.Reservation.Include(i => i.Venue).Where(i => i.Venue.ProviderId == currentUserId.Value && i.PaymentStatus == "Approved");
                }
                else if (status == "Cancelled")
                {
                    ReservationList = db.Reservation.Include(i => i.Venue).Where(i => i.Venue.ProviderId == currentUserId.Value && i.Status == status);
                }
                else
                {
                    ReservationList = db.Reservation.Include(i => i.Venue).Where(i => i.Venue.ProviderId == currentUserId.Value && i.Status == status && i.PaymentStatus=="Pending");
                }
            }
            else if (user.Role == "Admin")
            {
                if (status == "All")
                {
                    ReservationList = db.Reservation;
                }
                else if (status == "Paid")
                {
                    ReservationList = db.Reservation.Where(i=>i.PaymentStatus == "Approved" && i.PaymentStatus == "Pending");
                }
                else
                {
                    ReservationList = db.Reservation.Where(i=>i.Status == status);
                }
            }
           
            
            
            List<ReservationViewModel> ResList = new List<ReservationViewModel>();
            foreach(var rez in ReservationList)
            {
                
                var venue = db.Venue.Find(rez.VenueId);
                var provider = db.User.Find(venue.ProviderId);
                var even = db.Event.Find(rez.EventId);
                ReservationViewModel obj = new ReservationViewModel();
                obj.Venue = venue;
                obj.Reservation = rez;
                obj.Event = even;
                obj.Provider = provider;
                ResList.Add(obj);

            }


            return View("Index", ResList);


        }

        //GET
        [Authorize(Roles = "Client")]
        public IActionResult Show(int? venueId, string? start, string? end)
        {

            if (venueId == null)
            {
                return NotFound();
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<SelectListItem> EventList = db.Event.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.EventId.ToString()
            });
            ViewBag.EventTypes = EventList;

            var venue = db.Venue.Find(venueId);
            var provider = db.User.Where(i => i.Id == venue.ProviderId).FirstOrDefault();
            var user = db.User.Where(i => i.Id == currentUserId.Value).FirstOrDefault();
            var address = db.Address.Where(i => i.AddressId == venue.AddressId).FirstOrDefault();

            IEnumerable<Calendar> CalendarList = db.Calendar.Where(i => i.VenueId == venueId);
            Reservation rez = new Reservation();
            rez.VenueId = venueId;
            rez.ClientEmail = user.Email;
            rez.ClientPhoneNumber = user.PhoneNumber;
            rez.ClientName = user.Name;
            rez.ClientId = currentUserId.Value;
            rez.StartDate = start;
            rez.EndDate = end;
            rez.TotalPrice = CalculatePrice(start, end, venueId);

            ReservationViewModel obj = new ReservationViewModel();
            obj.Venue = venue;
            obj.Address = address;
            obj.Provider = provider;
            obj.Reservation = rez;
            obj.CalendarList = CalendarList;
            return View(obj);
        }

        //POST
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReservationViewModel obj)
        {

            if (ModelState.IsValid)
            {

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


                obj.Reservation.ClientId = currentUserId.Value;
                obj.Reservation.VenueId = obj.Venue.VenueId;
                obj.Reservation.Status = "Pending";
                obj.Reservation.PaymentStatus = "Pending";
                db.Reservation.Add(obj.Reservation);
                db.SaveChanges();
                var available = false;
                UpdateCalendar(obj.Reservation.StartDate, obj.Reservation.EndDate, obj.Reservation.VenueId, available);
                TempData["Success"] = "Your reservation is pending!";
                return RedirectToAction("Index", "Reservation");

            }

            return View("Show", obj);
        }
        //POST
        [Authorize(Roles = "Provider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReservationApproval(ReservationViewModel obj)
        {
            

            var reservation = db.Reservation.Where(i => i.ReservationId == obj.Reservation.ReservationId).FirstOrDefault();
            reservation.Status = "Approved";
            db.Reservation.Update(reservation);
            db.SaveChanges();

            TempData["Success"] = "You have approved a reservation!";
            return RedirectToAction("Index", "Reservation");

        }

        //POST
        [Authorize(Roles = "Provider,Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReservationCancellation(ReservationViewModel obj)
        {
            

            var reservation = db.Reservation.Where(i => i.ReservationId == obj.Reservation.ReservationId).FirstOrDefault();
            reservation.Status = "Cancelled";
            db.Reservation.Update(reservation);
            db.SaveChanges();
            var available = true;
            UpdateCalendar(reservation.StartDate, reservation.EndDate, reservation.VenueId, available);

            TempData["Success"] = "You have cancelled a reservation!";
            return RedirectToAction("Index", "Reservation");

        }

        //POST
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReservationPayment(ReservationViewModel obj)
        {

            var res = db.Reservation.Where(i => i.ReservationId == obj.Reservation.ReservationId).FirstOrDefault();
            var venue = db.Venue.Find(res.VenueId);
            //Stripe Settings
            var domain = "https://localhost:7097/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"Client/Reservation/ReservationConfirmation?id={res.ReservationId}",
                CancelUrl = domain + $"Client/Reservation/Index",
            };


            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {

                    UnitAmount = (long)(res.TotalPrice * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = venue.Name,
                    },

                },
                Quantity = 1,
            };
            options.LineItems.Add(sessionLineItem);

            var service = new SessionService();
            Session session = service.Create(options);
            res.SessionId = session.Id;
            res.PaymentIntentId = session.PaymentIntentId;
            db.Reservation.Update(res);
            db.SaveChanges();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }
        [Authorize(Roles = "Provider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReservationPaymentRefund(ReservationViewModel obj)
        {

            var reservation = db.Reservation.FirstOrDefault(i => i.ReservationId == obj.Reservation.ReservationId);

            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = reservation.PaymentIntentId,
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            reservation.PaymentStatus = "Cancelled";
            reservation.Status = "Cancelled";
            db.Reservation.Update(reservation);
            db.SaveChanges();
            var available = true;
            UpdateCalendar(reservation.StartDate, reservation.EndDate, reservation.VenueId, available);
            TempData["Success"] = "Reservation cancelled and payment refunded!";
            return RedirectToAction("Index", "Reservation");
        }

        [Authorize(Roles = "Client")]
        public IActionResult ReservationConfirmation(int id)
        {

            var reservation = db.Reservation.FirstOrDefault(i => i.ReservationId == id);

            var service = new SessionService();
            Session session = service.Get(reservation.SessionId);
            
            //check stripe status
            if(session.PaymentStatus.ToLower() == "paid")
            {
                reservation.Status = "Approved";
                reservation.PaymentStatus = "Approved";
                db.Reservation.Update(reservation);
                db.SaveChanges();
            }

            return View(id);
        }



    }
}
