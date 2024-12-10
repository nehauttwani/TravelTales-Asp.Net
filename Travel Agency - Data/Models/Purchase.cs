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
        public int PackageId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; } // Product or package name

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        // Optional navigation properties
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("PackageId")]
        public Package Package { get; set; }
    }
}
