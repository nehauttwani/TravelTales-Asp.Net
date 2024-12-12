using System.Collections.Generic;

namespace Travel_Agency___Data.ViewModels
{
    public class PurchasedProductsSummaryViewModel
    {
        public List<PurchasedProductViewModel> Products { get; set; } = new();
        public decimal TotalPaid { get; set; }
    }

    public class PurchasedProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
