using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Agency___Data.ViewModels
{
    public class ConfirmationViewModel
    {
        public string PackageName { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsPaid { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
    }
}
