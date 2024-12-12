using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Agency___Data.Models
{
    public class WalletTransaction
    {
        [Key]
        public int TransactionId { get; set; } // Primary Key

        [Required]
        public int CustomerId { get; set; } // Foreign Key to Customer table

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now; // Defaults to current date/time

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } // Transaction amount

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } // e.g., Deposit, Withdrawal

        [StringLength(255)]
        public string? Description { get; set; } // Optional description

        // Navigation property
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } // Navigation property to link to the Customer table
    }
}
