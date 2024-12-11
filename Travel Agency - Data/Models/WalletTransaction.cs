using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Agency___Data.Models
{
    public class WalletTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int WalletId { get; set; } // Foreign key to Wallet table

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } // Positive for deposits, negative for withdrawals

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } // Deposit, Withdrawal, etc.

        [StringLength(255)]
        public string Description { get; set; } // Optional description of the transaction

        // Navigation Property
        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; }
    }
}
