using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ViewModels
{
    public class WalletViewModel
    {
        public decimal CurrentBalance { get; set; }
        public int CustomerId { get; set; }
        public IEnumerable<CreditCard> CreditCards { get; set; }
        public IEnumerable<WalletTransaction> Transactions { get; set; }

        [Required(ErrorMessage = "Please select a card type")]
        [Display(Name = "Card Type")]
        public string Ccname { get; set; }

        [Required(ErrorMessage = "Card Number is required")]
        [Display(Name = "Card Number")]
        public string Ccnumber { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        [Display(Name = "Expiry Date")]
        public string Ccexpiry { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }

    // Custom validation attribute for future date
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime date = (DateTime)value;
            return date > DateTime.Now;
        }
    }
}