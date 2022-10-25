using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectLicenta.Models
{

    public class Venue
    {
        //Data Annotations
        [Key]
        public int VenueId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0, 2000)]
        public int Capacity { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;


        [DisplayName("Service Provider")]
        [ValidateNever]

        public string ProviderId { get; set; }

        [ForeignKey("ProviderId")]
        [ValidateNever]
        public User User { get; set; }


        [ValidateNever]
        public int AddressId { get; set; }


        [ForeignKey("AddressId")]
        [ValidateNever]
        public Address Address { get; set; }

        [ValidateNever]
        public virtual ICollection<Event>Event { get; set; }



    }
}
