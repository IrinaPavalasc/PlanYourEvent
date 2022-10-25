using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectLicenta.Models
{
    public class Calendar
    {
        [Key]
        public int CalendarId { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public bool Available { get; set; }

        [Required]
        public int PricePerDay { get; set; }

        [Required]
        [ValidateNever]
        public int VenueId { get; set; }

        [ForeignKey("VenueId")]
        [ValidateNever]
        Venue Venue { get; set; }
    }

}
