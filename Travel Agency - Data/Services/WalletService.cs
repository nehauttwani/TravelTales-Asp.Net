using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;
using System.Collections.Generic;

namespace Travel_Agency___Data.Services
{
    public class WalletService
    {
        private readonly TravelExpertsContext _context;

        public WalletService(TravelExpertsContext context)
        {
            _context = context;
        }

        // Retrieve wallet balance for a specific customer
        public async Task<decimal> GetWalletBalanceAsync(int customerId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            return wallet?.Balance ?? 0m;
        }

        // Deduct funds from the wallet
        public async Task<bool> DeductFundsAsync(int customerId, decimal amount)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (wallet != null && wallet.Balance >= amount)
            {
                wallet.Balance -= amount;

                // Log the transaction
                var transaction = new WalletTransaction
                {
                    CustomerId = customerId,
                    Amount = -amount,
                    TransactionType = "Withdrawal",
                    Description = "Funds deducted",
                    TransactionDate = DateTime.Now
                };
                _context.WalletTransactions.Add(transaction);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Add funds to the wallet
        public async Task<bool> AddFundsAsync(int customerId, decimal amount)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    CustomerId = customerId,
                    Balance = 0
                };
                _context.Wallets.Add(wallet);
            }

            wallet.Balance += amount;

            // Log the transaction
            var transaction = new WalletTransaction
            {
                CustomerId = customerId,
                Amount = amount,
                TransactionType = "Deposit",
                Description = "Funds added via credit card",
                TransactionDate = DateTime.Now
            };
            _context.WalletTransactions.Add(transaction);

            await _context.SaveChangesAsync();
            return true;
        }

        // Fetch wallet transactions for a customer
        public async Task<List<WalletTransaction>> GetWalletTransactionsAsync(int customerId)
        {
            return await _context.WalletTransactions
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        // Fetch credit cards for a customer
        public async Task<List<CreditCard>> GetCreditCardsAsync(int customerId)
        {
            return await _context.CreditCards
                .Where(cc => cc.CustomerId == customerId)
                .ToListAsync();
        }

        // Process credit card payment (mocked functionality)
        public async Task<bool> ProcessCreditCardPayment(int customerId, int creditCardId, decimal amount)
        {
            var creditCard = await _context.CreditCards.FirstOrDefaultAsync(c => c.CreditCardId == creditCardId && c.CustomerId == customerId);
            if (creditCard == null)
            {
                return false;
            }

            // Simulate payment success (this can be replaced with actual payment gateway logic)
            return true;
        }
    }
}
