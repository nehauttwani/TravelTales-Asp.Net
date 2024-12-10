using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data;
using Travel_Agency___Data.ViewModels;
{
    public class WalletController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;


namespace Travel_Agency___Web.Controllers
{
    public class WalletController : Controller
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }

        // View wallet details (balance, transactions, credit cards)
        public async Task<IActionResult> Index(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            try
            {
                // Fetch wallet details and associated credit cards
                var walletDetails = await _walletService.GetWalletDetailsAsync(customerId);
                var creditCards = await _walletService.GetCreditCardsForCustomerAsync(customerId);

                var viewModel = new WalletViewModel
                {
                    CustomerId = customerId,
                    CurrentBalance = walletDetails?.CurrentBalance ?? 0,
                    Transactions = walletDetails?.Transactions ?? new List<TransactionViewModel>(),
                    CreditCards = creditCards
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                Console.WriteLine($"Error fetching wallet details: {ex.Message}");
                return NotFound($"Unable to find wallet details for Customer ID: {customerId}");
            }
        }

        // Add funds via credit card
        [HttpPost]
        public async Task<IActionResult> AddFundsWithCreditCard(int customerId, int creditCardId, decimal amount)
        {
            if (customerId <= 0 || creditCardId <= 0 || amount <= 0)
            {
                return BadRequest("Invalid input. Please provide valid customer ID, credit card ID, and amount.");
            }

            var paymentSuccess = await _walletService.ProcessCreditCardPayment(customerId, creditCardId, amount);
            if (!paymentSuccess)
            {
                return BadRequest("Failed to process the payment. Please check the credit card details.");
            }

            await _walletService.AddFundsAsync(customerId, amount);
            return RedirectToAction("Index", new { customerId });
        }

        // Deduct funds from wallet
        [HttpPost]
        public async Task<IActionResult> DeductFunds(int customerId, decimal amount)
        {
            if (customerId <= 0 || amount <= 0)
            {
                return BadRequest("Invalid input. Please provide a valid customer ID and amount.");
            }

            var result = await _walletService.DeductFundsAsync(customerId, amount);
            if (!result)
            {
                return BadRequest("Insufficient funds.");
            }

            return RedirectToAction("Index", new { customerId });
        }

        // Optional: View associated credit cards
        public async Task<IActionResult> CreditCards(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            var creditCards = await _walletService.GetCreditCardsForCustomerAsync(customerId);
            if (creditCards == null || !creditCards.Any())
            {
                return NotFound($"No credit card data found for customer ID {customerId}.");
            }

            return View(creditCards);
        }
    }
}
