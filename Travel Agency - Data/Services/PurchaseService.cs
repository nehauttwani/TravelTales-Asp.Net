using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.Services
{
    public class PurchaseService
    {
        private readonly TravelExpertsContext _context;

        public PurchaseService(TravelExpertsContext context)
        {
            _context = context;
        }

        public async Task<Package> GetPackageAsync(int packageId)
        {
            return await _context.Packages
                .FirstOrDefaultAsync(p => p.PackageId == packageId);
        }

        public async Task SavePurchaseAsync(Purchase purchase)
        {
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
        }
    }
}