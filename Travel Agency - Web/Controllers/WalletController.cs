using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data;
using Travel_Agency___Data.Services;
using Travel_Agency___Web.Models;

namespace Travel_Agency___Web.Controllers
{
    public class WalletController : Controller
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }

        // View wallet details (balance, transactions, and associated credit cards)
        public async Task<IActionResult> Index(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            try
            {
                var walletDetails = await _walletService.GetWalletDetailsAsync(customerId);
                var creditCards = await _walletService.GetCreditCardsForCustomerAsync(customerId);

                var viewModel = new WalletViewModel
                {
                    CustomerId = walletDetails.CustomerId,
                    CurrentBalance = walletDetails.CurrentBalance,
                    Transactions = walletDetails.Transactions,
                    CreditCards = creditCards // Include credit cards in the view model
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Handle cases where customer or wallet details are not found
            }
        }

        // Add funds to wallet
        [HttpPost]
        public async Task<IActionResult> AddFunds(int customerId, decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            await _walletService.AddFundsAsync(customerId, amount);

            // Redirect to the wallet index page
            return RedirectToAction("Index", new { customerId });
        }

        // Deduct funds from wallet
        [HttpPost]
        public async Task<IActionResult> DeductFunds(int customerId, decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            var result = await _walletService.DeductFundsAsync(customerId, amount);

            if (!result)
            {
                return BadRequest("Insufficient funds.");
            }

            // Redirect to the wallet index page
            return RedirectToAction("Index", new { customerId });
        }

        // View credit cards associated with the customer
        public async Task<IActionResult> CreditCards(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            // Fetch credit card details for the customer
            var creditCards = await _walletService.GetCreditCardsForCustomerAsync(customerId);

            if (creditCards == null || !creditCards.Any())
            {
                return NotFound($"No credit card data found for customer ID {customerId}.");
            }

            return View(creditCards);
        }

        // Add funds using a credit card
        [HttpPost]
        public async Task<IActionResult> AddFundsWithCreditCard(int customerId, int creditCardId, decimal amount)
        {
            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            var paymentSuccess = await _walletService.ProcessCreditCardPaymentAsync(customerId, creditCardId, amount);
            if (!paymentSuccess)
            {
                return BadRequest("Failed to process the payment. Please check the credit card details.");
            }

            return RedirectToAction("Index", new { customerId });
        }
    }
}
