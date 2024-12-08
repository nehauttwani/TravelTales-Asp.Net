using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Agency___Data.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasePrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Tax { get; set; }

        public decimal TotalPrice => BasePrice + Tax;
        public bool IsPaid { get; set; }

        public DateTime PurchaseDate { get; set; }


        // Navigation property (optional)
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
