﻿using System;
using System.Collections.Generic;

namespace Travel_Agency___Data.ViewModels
{
    public class WalletViewModel
    {
        // Customer ID associated with the wallet
        public int CustomerId { get; set; }

        // Current balance of the wallet
        public decimal CurrentBalance { get; set; }

        // List of wallet transactions
        public List<TransactionViewModel> Transactions { get; set; } = new();

        // List of credit cards associated with the customer
        public List<CreditCardViewModel> CreditCards { get; set; } = new();
    }

    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty; // e.g., "Deposit", "Withdrawal"
    }

    public class CreditCardViewModel
    {
        public int CreditCardId { get; set; }
        public string Ccnumber { get; set; } = string.Empty; // Mask card numbers when displaying
        public string Ccname { get; set; } = string.Empty;
        public DateTime Ccexpiry { get; set; }
    }
}