using System.Threading.Tasks;
using Travel_Agency___Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Travel_Agency___Data.Services
{
    public class WalletService
    {
        private readonly TravelExpertsContext _context;

        public WalletService(TravelExpertsContext context)
        {
            _context = context;
        }

        // Fetch wallet information for a customer
        public async Task<Wallet> GetWalletAsync(int customerId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.CustomerId == customerId);
        }

        // Add funds to a wallet
        public async Task<bool> AddFundsAsync(int customerId, decimal amount)
        {
            var wallet = await GetWalletAsync(customerId);
            if (wallet == null)
                return false;

            wallet.Balance += amount;
            await _context.SaveChangesAsync();
            return true;
        }

        // Deduct funds from a wallet
        public async Task<bool> DeductFundsAsync(int customerId, decimal amount)
        {
            var wallet = await GetWalletAsync(customerId);
            if (wallet == null || wallet.Balance < amount)
                return false;

            wallet.Balance -= amount;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
