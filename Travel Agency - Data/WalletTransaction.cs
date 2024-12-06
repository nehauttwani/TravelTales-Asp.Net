using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data
{
    public class WalletTransaction
    {
        public int TransactionId { get; set; } // Primary Key
        public int CustomerId { get; set; }    // Foreign Key referencing Customer
        public DateTime TransactionDate { get; set; } = DateTime.Now; // Default to now
        public decimal Amount { get; set; }   // Transaction Amount
        public string TransactionType { get; set; } // Type: AddFunds or Payment

        // Navigation Property
        public virtual Customer Customer { get; set; }
    }
}
