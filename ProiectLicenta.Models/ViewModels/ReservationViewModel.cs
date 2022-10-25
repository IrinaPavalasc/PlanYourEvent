using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;
namespace ProiectLicenta.Models.ViewModels
{
    public class ReservationViewModel
    {
        [ValidateNever]
        public Venue Venue { get; set; }
        [ValidateNever]
        public Address Address{ get; set; }
        [ValidateNever]
        public Event Event { get; set; }
        [ValidateNever]
        public User Provider { get; set; }

        [ValidateNever]
        public User Client { get; set; }
        [ValidateNever]
        public Reservation Reservation { get; set; }
        [ValidateNever]

        public IEnumerable<Calendar> CalendarList { get; set; }


    }
}
