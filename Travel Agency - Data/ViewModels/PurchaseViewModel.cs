using System.ComponentModel.DataAnnotations;

using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ViewModels

{

    public class PurchaseViewModel

    {

        [Required]

        public int PackageId { get; set; } // ID of the selected package

        [Required]

        [Display(Name = "Package Name")]

        public string PackageName { get; set; } // Name of the selected package

        public string Description { get; set; } // Package description

        [Required]

        [DataType(DataType.Currency)]

        [Display(Name = "Price Per Person")]

        public decimal PricePerPerson { get; set; } // Price per traveler

        [Required]

        [Range(1, int.MaxValue, ErrorMessage = "Please enter at least 1 traveler.")]

        [Display(Name = "Number of Travelers")]

        public int TravelerCount { get; set; } // Number of travelers

        [Required]

        [DataType(DataType.Currency)]

        [Display(Name = "Total Price")]

        public decimal TotalPrice { get; set; } // Total cost for the purchase

        [Required]

        public int CustomerId { get; set; } // ID of the customer

        [DataType(DataType.Currency)]

        [Display(Name = "Wallet Balance")]

        public decimal WalletBalance { get; set; } // Current wallet balance of the customer

        public IEnumerable<CreditCard> CreditCards { get; set; } // Add this property

        public decimal Price { get; set; } // Add this to hold the package price

    }

}

