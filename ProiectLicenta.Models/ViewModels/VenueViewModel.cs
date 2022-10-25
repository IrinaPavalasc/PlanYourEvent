using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Web.WebPages.Html;

namespace ProiectLicenta.Models.ViewModels
{
    public class VenueViewModel
    {
        [Required]
        public Venue Venue { get; set; }
        [Required]
        public Address Address { get; set; }

        [ValidateNever]
        public User User { get; set; }

        [ValidateNever]
        public Calendar Calendar{ get; set; }
        [ValidateNever]
        public string StartDate { get; set; }

        [ValidateNever]
        public string EndDate { get; set; }

        [ValidateNever]
        public string Search { get; set; }
        [ValidateNever]

        public IEnumerable<Calendar> CalendarList { get; set; }

        [ValidateNever]
        public IList<SelectListItem> Events { get; set; }
    }
}
