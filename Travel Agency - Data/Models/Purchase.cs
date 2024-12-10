using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travel_Agency___Data.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        public int CustomerId { get; set; }
        public int PackageId { get; set; }
        public string ProductName { get; set; } // Product or package name
        public decimal Tax { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsPaid { get; set; }
    }
}
