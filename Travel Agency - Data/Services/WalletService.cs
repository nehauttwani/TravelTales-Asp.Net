using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.Services
{
    public class WalletService
    {
        private readonly TravelExpertsContext _context;

        public WalletService(TravelExpertsContext context)
        {
            _context = context;
        }

        // Retrieve the wallet balance for a specific customer
        public async Task<decimal> GetWalletBalanceAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            return customer?.CreditBalance ?? 0m; // Return 0 if customer is not found
        }

        // Deduct funds from the wallet
        public async Task<bool> DeductFundsAsync(int customerId, decimal amount)
        {
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer != null && customer.CreditBalance >= amount)
            {
                customer.CreditBalance -= amount;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        // Add funds to the wallet
        public async Task<bool> AddFundsAsync(int customerId, decimal amount)
        {
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer != null)
            {
                customer.CreditBalance += amount;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
