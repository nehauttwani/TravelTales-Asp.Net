using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Travel_Agency___Data.Services
{
    public class WalletService
    {
        private readonly TravelExpertsContext _context;
        private readonly ILogger<WalletService> _logger;


        public WalletService(TravelExpertsContext context, ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
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
        public async Task<bool> AddFundsAsync(int customerId, int creditCardId, decimal amount)
        {
            try
            {
                // Validate the amount
                if (amount < 10.00m || amount > 10000.00m)
                {
                    _logger.LogWarning("Invalid amount attempted: {Amount} for CustomerId: {CustomerId}",
                        amount, customerId);
                    return false;
                }

                // First process the credit card payment
                var paymentSuccess = await ProcessCreditCardPayment(customerId, creditCardId, amount);
                if (!paymentSuccess)
                {
                    _logger.LogWarning("Credit card payment failed for CustomerId: {CustomerId}", customerId);
                    return false;
                }

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
                _logger.LogInformation("Successfully added {Amount} to wallet for CustomerId: {CustomerId}",
                    amount, customerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding funds for CustomerId: {CustomerId}", customerId);
                return false;
            }
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

        public async Task<bool> AddCreditCardAsync(CreditCard creditCard)
        {
            try
            {
                // Enhanced validation
                if (string.IsNullOrEmpty(creditCard.Ccnumber) ||
                    string.IsNullOrEmpty(creditCard.Ccname))
                {
                    _logger.LogWarning("Invalid credit card data for CustomerId: {CustomerId}",
                        creditCard.CustomerId);
                    return false;
                }

                if (creditCard.Ccexpiry < DateTime.Now)
                {
                    _logger.LogWarning("Expired credit card attempted for CustomerId: {CustomerId}",
                        creditCard.CustomerId);
                    return false;
                }

                // Check for existing cards
                var existingCard = await _context.CreditCards
                    .AnyAsync(cc => cc.CustomerId == creditCard.CustomerId);

                if (existingCard)
                {
                    _logger.LogWarning("Customer already has a credit card. CustomerId: {CustomerId}",
                        creditCard.CustomerId);
                    return false;
                }

                // Clean card number (remove spaces)
                creditCard.Ccnumber = creditCard.Ccnumber.Replace(" ", "");

                _context.CreditCards.Add(creditCard);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Credit card added successfully for CustomerId: {CustomerId}",
                    creditCard.CustomerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding credit card for CustomerId: {CustomerId}",
                    creditCard.CustomerId);
                return false;
            }
        }

        public async Task<bool> DeleteCreditCardAsync(int customerId, int creditCardId)
        {
            try
            {
                var creditCard = await _context.CreditCards
                    .FirstOrDefaultAsync(cc => cc.CreditCardId == creditCardId &&
                                             cc.CustomerId == customerId);

                if (creditCard == null)
                {
                    _logger.LogWarning("Credit card not found. CustomerId: {CustomerId}, CreditCardId: {CreditCardId}",
                        customerId, creditCardId);
                    return false;
                }

                _context.CreditCards.Remove(creditCard);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Credit card deleted successfully. CustomerId: {CustomerId}, CreditCardId: {CreditCardId}",
                    customerId, creditCardId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting credit card. CustomerId: {CustomerId}, CreditCardId: {CreditCardId}",
                    customerId, creditCardId);
                return false;
            }
        }
    }
}