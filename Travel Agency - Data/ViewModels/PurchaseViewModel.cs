using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Travel_Agency___Data.ViewModels
{
    public class PurchaseViewModel
    {
        [Required]
        public int PackageId { get; set; } // ID of the selected package

        [Required]
        [Display(Name = "Package Name")]
        public string PackageName { get; set; } // Name of the selected package

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public decimal Price { get; set; } // Price of the package

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Price Per Person")]
        public decimal PricePerPerson { get; set; }
        [Required]
        public int CustomerId { get; set; } // ID of the customer

        [DataType(DataType.Currency)]
        [Display(Name = "Wallet Balance")]
        public decimal WalletBalance { get; set; } // Current wallet balance of the customer

        [Display(Name = "Description")]
        public string Description { get; set; } // Package description

        [Display(Name = "Trip Start Date")]
        [DataType(DataType.Date)]
        public DateTime TripStart { get; set; } // Start date of the trip

        [Display(Name = "Trip End Date")]
        [DataType(DataType.Date)]
        public DateTime TripEnd { get; set; } // End date of the trip
    }
}
