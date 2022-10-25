using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProiectLicenta.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(100)]
        [Required]
        public string Country { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }

        [Required]
        [ValidateNever]
        [DisplayName("Type of Provider")]
        public string ProviderType { get; set; }

        [Required]
        public string Role { get; set; }


    }
}
