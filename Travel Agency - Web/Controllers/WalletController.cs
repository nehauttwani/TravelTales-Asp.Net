using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Travel_Agency___Web.Controllers
{
    public class WalletController : Controller
    {
        private readonly TravelExpertsContext _context;
        private readonly ILogger<WalletController> _logger;

        public WalletController(TravelExpertsContext context, ILogger<WalletController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int customerId)
        {
            _logger.LogInformation("WalletController.Index called with customerId: {CustomerId}", customerId);

            // Fetch wallet for the customer
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerId == customerId);

            if (wallet == null)
            {
                TempData["ErrorMessage"] = "Wallet not found for this customer.";
                return RedirectToAction("Purchase", "Purchase", new { customerId });
            }

            // Fetch customer's credit cards
            var creditCards = await _context.CreditCards
                .Where(cc => cc.CustomerId == customerId)
                .ToListAsync();

            // Fetch wallet transactions
            var transactions = await _context.WalletTransactions
                .Where(t => t.WalletId == wallet.WalletId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            // Prepare the view model
            var viewModel = new WalletViewModel
            {
                CustomerId = customerId,
                WalletBalance = wallet.Balance,
                CreditCards = creditCards,
                Transactions = transactions
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddFunds(int customerId, int creditCardId, decimal amount)
        {
            _logger.LogInformation("AddFunds called with customerId: {CustomerId}, creditCardId: {CreditCardId}, amount: {Amount}", customerId, creditCardId, amount);

            // Check if the customer exists
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("Index", new { customerId });
            }

            // Fetch or create wallet for the customer
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    CustomerId = customerId,
                    Balance = 0
                };
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            // Ensure customer has enough credit balance
            if (customer.CreditBalance < amount)
            {
                TempData["ErrorMessage"] = "Insufficient credit balance on your card.";
                return RedirectToAction("Index", new { customerId });
            }

            // Update wallet balance
            wallet.Balance += amount;

            // Deduct the amount from customer's CreditBalance
            customer.CreditBalance -= amount;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Log the transaction in WalletTransactions
            var transaction = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                Amount = amount,
                TransactionType = "Deposit",
                Description = $"Added funds via card ending in {creditCardId}"
            };
            _context.WalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Funds added successfully to your wallet.";
            return RedirectToAction("Index", new { customerId });
        }
    }
}
