using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;
using System.Threading.Tasks;

namespace Travel_Agency___Web.Controllers
{
    public class WalletController : Controller
    {
        private readonly WalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(WalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int customerId)
        {
            _logger.LogInformation("Fetching wallet details for CustomerId: {CustomerId}", customerId);

            var balance = await _walletService.GetWalletBalanceAsync(customerId);
            var creditCards = await _walletService.GetCreditCardsAsync(customerId);
            var transactions = await _walletService.GetWalletTransactionsAsync(customerId);

            var viewModel = new WalletViewModel
            {
                CustomerId = customerId,
                CurrentBalance = balance,
                CreditCards = creditCards,
                Transactions = transactions
            };

            return View(viewModel);
        }


        // Add funds via credit card
        [HttpPost]
        public async Task<IActionResult> AddFundsWithCreditCard(int customerId, int creditCardId, decimal amount)
        {
            _logger.LogInformation("Adding funds. CustomerId: {CustomerId}, CreditCardId: {CreditCardId}, Amount: {Amount}", customerId, creditCardId, amount);

            if (customerId <= 0 || creditCardId <= 0 || amount <= 0)
            {
                TempData["ErrorMessage"] = "Invalid input. Please provide valid customer ID, credit card ID, and amount.";
                return RedirectToAction("Index", new { customerId });
            }

            var paymentSuccess = await _walletService.ProcessCreditCardPayment(customerId, creditCardId, amount);
            if (!paymentSuccess)
            {
                TempData["ErrorMessage"] = "Failed to process the payment. Please check the credit card details.";
                return RedirectToAction("Index", new { customerId });
            }

            var fundsAdded = await _walletService.AddFundsAsync(customerId, amount);
            if (fundsAdded)
            {
                TempData["SuccessMessage"] = "Funds added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add funds to the wallet.";
            }

            return RedirectToAction("Index", new { customerId });
        }
    }
}
