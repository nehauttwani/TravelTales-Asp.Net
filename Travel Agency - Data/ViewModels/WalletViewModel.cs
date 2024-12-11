using System;
using System.Collections.Generic;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ViewModels
{
    public class WalletViewModel
    {
        public int CustomerId { get; set; } // ID of the customer
        public decimal WalletBalance { get; set; } // Current wallet balance
        public IEnumerable<CreditCard> CreditCards { get; set; } // List of customer's credit cards
        public IEnumerable<WalletTransaction> Transactions { get; set; } // List of wallet transactions
    }

    //    public class TransactionViewModel
    //    {
    //        public int TransactionId { get; set; }
    //        public DateTime TransactionDate { get; set; }
    //        public decimal Amount { get; set; }
    //        public string TransactionType { get; set; } = string.Empty; // e.g., "Deposit", "Withdrawal"
    //    }

    //    public class CreditCardViewModel
    //    {
    //        public int CreditCardId { get; set; }
    //        public string Ccnumber { get; set; } = string.Empty; // Mask card numbers when displaying
    //        public string Ccname { get; set; } = string.Empty;
    //        public DateTime Ccexpiry { get; set; }
    //    }
}
