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
            // Query the Wallets table for the wallet balance
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (wallet == null)
            {
                Console.WriteLine($"Wallet not found for Customer ID {customerId}.");
                return 0m; // Return 0 if no wallet exists for the customer
            }

            Console.WriteLine($"Wallet Balance for Customer {customerId}: {wallet.Balance}");
            return wallet.Balance; // Return the wallet balance
        }

        // Deduct funds from the wallet
        public async Task<bool> DeductFundsAsync(int customerId, decimal amount)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (wallet != null && wallet.Balance >= amount)
            {
                wallet.Balance -= amount; // Deduct the amount (including tax)
                await _context.SaveChangesAsync();
                return true;
            }

            return false; // Insufficient funds
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
