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
   public class SearchViewModel
    {
        
        public IEnumerable<VenueViewModel> VenueList;
        [ValidateNever]
        public string? StartDate { get; set; }
        [ValidateNever]
        public string? EndDate { get; set; }
        [ValidateNever]
        [StringLength(100)]
        public string? Search { get; set; }

        [Range(1,2000)]
        public int? MaxCapacity { get; set; }
        [ValidateNever]
        public string? Country { get; set; }
        [ValidateNever]
        public string? City { get; set; }

        [ValidateNever]
        public IList<SelectListItem> Events { get; set; }
    }
}
