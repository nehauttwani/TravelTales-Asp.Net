using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Agency___Data.Models
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [Column(TypeName = "money")] // Match database column type
        public decimal Balance { get; set; }

        // Navigation property (optional, if used)
      //  public virtual Customer Customer { get; set; }
    }
}
