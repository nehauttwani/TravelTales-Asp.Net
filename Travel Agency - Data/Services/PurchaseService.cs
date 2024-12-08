using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Data.Services
{
    public class PurchaseService
    {
        private readonly TravelExpertsContext _context;

        public PurchaseService(TravelExpertsContext context)
        {
            _context = context;
        }

        // Fetch purchased products for a customer
        public async Task<List<PurchasedProductViewModel>> GetPurchasedProductsAsync(int customerId)
        {
            return await _context.Purchases
       .Where(p => p.CustomerId == customerId)
       .Select(p => new PurchasedProductViewModel
       {
           ProductName = p.ProductName,
           BasePrice = p.BasePrice,
           Tax = p.Tax,
           TotalPrice = p.BasePrice + p.Tax,
           PurchaseDate = p.PurchaseDate // Include PurchaseDate
       })
       .ToListAsync();
        }

        // Get the outstanding balance for a customer
        public async Task<decimal> GetOutstandingBalanceAsync(int customerId)
        {
            return await _context.Purchases
                .Where(p => p.CustomerId == customerId && !p.IsPaid)
                .SumAsync(p => p.BasePrice + p.Tax);
        }
    }
}
