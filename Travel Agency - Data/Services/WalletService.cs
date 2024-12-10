using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;

namespace Travel_Agency___Data.Services
{
    public class WalletService : IWalletService
    {
        private readonly TravelExpertsContext _context;

        public WalletService(TravelExpertsContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetWalletBalanceAsync(int customerId)
        {
            return await _context.Customers
                .Where(c => c.CustomerId == customerId)
                .Select(c => c.CreditBalance)
                .FirstOrDefaultAsync();
        }

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
