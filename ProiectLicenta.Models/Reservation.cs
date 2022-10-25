using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProiectLicenta.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        public string? ClientId{ get; set; }
        [ForeignKey("ClientId")]
        [ValidateNever]
        public User User { get; set; }

        public int? VenueId { get; set; }
        [ForeignKey("VenueId")]
        [ValidateNever]
        public Venue Venue { get; set; }

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        [ValidateNever]
        public Event Event { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string EndDate { get; set; }

        [Required]
        public int TotalPrice { get; set; }

        public string? Status { get; set; }

        [Required]
        [StringLength(100)]
        public string ClientName { get; set; }

        [Required]
        [EmailAddress]
        public string ClientEmail { get; set; }

        [Required]
        [Phone]
        public string ClientPhoneNumber { get; set; }

        [Required]
        [Range(1, 2000)]
        public int GuestNumber { get; set; }

        public string? PaymentStatus { get; set; }
        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }
    }


}

