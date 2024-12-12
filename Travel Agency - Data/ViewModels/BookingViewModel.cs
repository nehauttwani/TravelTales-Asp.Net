using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ViewModels
{
    public class BookingViewModel
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }

        public string? PackageImage{ get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public string BookingNo { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Traveler count must be at least 1")]
        public int TravelerCount { get; set; }

        public int? CustomerId { get; set; }

        public string? ClassId { get; set; }
        public List<Class>? Classes { get; set; }
        public string? TripTypeId { get; set; }
        public List<TripType>? TripTypes { get; set; }
       



        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd, hh.mm tt}")]
        public DateTime TripStart { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TripEnd { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }
        public string? Destination { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal AgencyCommission { get; set; }

        public int? ProductSupplierId { get; set; }

        public int? BookingId { get; set; }
    }
}
