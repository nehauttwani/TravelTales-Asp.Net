using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;
using Travel_Agency___Web.Models;

namespace Travel_Agency___Data.Services
{
    public class WalletService
    {
        private readonly TravelExpertsContext _context;

        public WalletService(TravelExpertsContext context)
        {
            _context = context;
        }

        // Retrieve all wallet transactions for a customer
        public async Task<List<TransactionViewModel>> GetTransactionsAsync(int customerId)
        {
            var transactions = await _context.WalletTransactions
                .Where(w => w.CustomerId == customerId)
                .OrderByDescending(w => w.TransactionDate)
                .ToListAsync();

            return transactions.Select(t => new TransactionViewModel
            {
                TransactionId = t.TransactionId,
                TransactionDate = t.TransactionDate,
                Amount = t.Amount,
                TransactionType = t.TransactionType
            }).ToList();
        }

        // Method to calculate wallet balance
        public async Task<decimal> GetWalletBalanceAsync(int customerId)
        {
            var transactions = _context.WalletTransactions
                .Where(t => t.CustomerId == customerId);

            var balance = await transactions.SumAsync(t =>
                t.TransactionType == "Deposit" ? t.Amount :
                t.TransactionType == "Withdrawal" ? -t.Amount : 0);

            return balance;
        }

        // Get wallet details for the customer (transactions and balance)
        public async Task<WalletViewModel> GetWalletDetailsAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                throw new Exception($"Customer with ID {customerId} not found.");
            }

            var transactions = await GetTransactionsAsync(customerId);

            return new WalletViewModel
            {
                CustomerId = customerId,
                CurrentBalance = customer.CreditBalance,
                Transactions = transactions
            };
        }

        // Process a credit card payment and add funds to wallet
        public async Task<bool> ProcessCreditCardPaymentAsync(int customerId, int creditCardId, decimal amount)
        {
            var creditCard = await _context.CreditCards
                .FirstOrDefaultAsync(cc => cc.CreditCardId == creditCardId && cc.CustomerId == customerId);

            if (creditCard == null)
            {
                return false; // Credit card not found
            }

            // Simulate a payment gateway interaction
            Console.WriteLine($"Processing payment of {amount:C} using credit card ending in {creditCard.Ccnumber.Substring(creditCard.Ccnumber.Length - 4)}");

            // Add funds to wallet
            await AddFundsAsync(customerId, amount);
            return true; // Simulate successful payment
        }

        // Retrieve credit cards associated with a customer
        public async Task<List<CreditCardViewModel>> GetCreditCardsForCustomerAsync(int customerId)
        {
            var creditCards = await _context.CreditCards
                .Where(cc => cc.CustomerId == customerId)
                .ToListAsync();

            return creditCards.Select(cc => new CreditCardViewModel
            {
                CreditCardId = cc.CreditCardId,
                Ccname = cc.Ccname,
                Ccnumber = "**** " + cc.Ccnumber.Substring(cc.Ccnumber.Length - 4), // Mask the number
                Ccexpiry = cc.Ccexpiry
            }).ToList();
        }

        // Add funds to a customer's wallet
        public async Task AddFundsAsync(int customerId, decimal amount)
        {
            var transaction = new WalletTransaction
            {
                CustomerId = customerId,
                Amount = amount,
                TransactionType = "Deposit",
                TransactionDate = DateTime.Now
            };

            _context.WalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null)
            {
                customer.CreditBalance += amount;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }
        }

        // Deduct funds from a customer's wallet
        public async Task<bool> DeductFundsAsync(int customerId, decimal amount)
        {
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null || customer.CreditBalance < amount)
            {
                return false; // Insufficient funds
            }

            customer.CreditBalance -= amount;

            var transaction = new WalletTransaction
            {
                CustomerId = customerId,
                Amount = -amount,
                TransactionType = "Withdrawal",
                TransactionDate = DateTime.Now
            };

            _context.WalletTransactions.Add(transaction);
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
