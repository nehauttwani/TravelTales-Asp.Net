using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data.Services;

using Travel_Agency___Web;

namespace Travel_Agency___Web.Controllers
{
    public class WalletController : Controller
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }

        // View wallet details (balance and transactions)
        public async Task<IActionResult> Index(int customerId)
        {
            try
            {
                var walletDetails = await _walletService.GetWalletDetailsAsync(customerId);
                return View(walletDetails);
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine(ex.Message);
                return NotFound($"Unable to find wallet details for Customer ID: {customerId}");
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
            var creditCards = await _walletService.GetCreditCardsAsync(customerId);

            if (creditCards == null || !creditCards.Any())
            {
                return NotFound($"No credit card data found for customer ID {customerId}.");
            }

            return View(creditCards);
        }
    }
}
