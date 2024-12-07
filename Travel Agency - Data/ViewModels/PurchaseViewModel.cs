using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Agency___Data.ViewModels
{
    public class PurchaseViewModel
    {
        public int CustomerId { get; set; }
        public int PackageId { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal WalletBalance { get; set; }
    }
}
