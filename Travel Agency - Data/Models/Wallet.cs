using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Agency___Data.Models
{
    public class Wallet
    {
        [Key] // Primary key
        public int WalletId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        // Optional navigation property if Customer is a related table
        // public Customer Customer { get; set; }
    }
}
