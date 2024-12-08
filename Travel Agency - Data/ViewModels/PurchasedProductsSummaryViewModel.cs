using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Agency___Data.ViewModels
{
    public class PurchasedProductViewModel
    {
        public string ProductName { get; set; } = string.Empty; // Name of the product or package
        public decimal BasePrice { get; set; } // Base price of the product/package
        public decimal Tax { get; set; } // Taxes applied
        public decimal TotalPrice { get; set; } // Total price including tax
        public DateTime PurchaseDate { get; set; }
    }

    public class PurchasedProductsSummaryViewModel
    {
        public List<PurchasedProductViewModel> Products { get; set; } = new(); // List of purchased products
        public decimal TotalPaid { get; set; } // Total amount paid
        public decimal OutstandingBalance { get; set; } // Any balance remaining to be paid
    }
}
