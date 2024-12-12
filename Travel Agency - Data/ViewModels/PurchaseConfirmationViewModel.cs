using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;


namespace Travel_Agency___Data.ViewModels
{
    public class PurchaseConfirmationViewModel
    {
        public int PurchaseId { get; set; } // Add this property
        public string PackageName { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
    }
}