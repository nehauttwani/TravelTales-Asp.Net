using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Agency___Data.ViewModels
{
    public class ProductListViewModel
    {
            
        public string PackageName { get; set; } // Name of the purchased travel package
        public decimal PackagePrice { get; set; } // Base price of the travel package
        public decimal AgencyCommission { get; set; } // Agency commission
        public decimal TotalCost => PackagePrice + AgencyCommission; // Total cost calculation
    }
}

