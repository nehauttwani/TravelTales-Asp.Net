using System;
using System.Collections.Generic;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ViewModels
{
    public class WalletViewModel
    {
        public int CustomerId { get; set; } // To identify the customer
        public decimal CurrentBalance { get; set; } // Wallet's current balance
        public List<CreditCard> CreditCards { get; set; } = new List<CreditCard>(); // Customer's credit cards
        public List<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>(); // Wallet transactions

        
        // Optional calculated properties for convenience
        public bool HasCreditCards => CreditCards?.Count > 0; // Check if the customer has any credit cards
        public bool HasTransactions => Transactions?.Count > 0; // Check if the wallet has any transactions

       
        // Optional display-friendly property for transaction count
        public string TransactionSummary => $"{Transactions?.Count ?? 0} transactions available.";
    }
}
