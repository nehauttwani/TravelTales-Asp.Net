using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Logging;

using Travel_Agency___Data.Models;

using Travel_Agency___Data.Services;

using Travel_Agency___Data.ViewModels;

using System.Threading.Tasks;
using System.Globalization;

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

        [HttpPost]
        public async Task<IActionResult> AddCreditCard(int customerId, string Ccname, string Ccnumber, string Ccexpiry)
        {
            try
            {
                // Check if customer already has a credit card
                var existingCard = await _walletService.GetCreditCardsAsync(customerId);
                if (existingCard != null && existingCard.Any())
                {
                    TempData["ErrorMessage"] = "You already have a credit card registered.";
                    return RedirectToAction("Index", new { customerId });
                }

                // Validate inputs
                if (string.IsNullOrEmpty(Ccname) || string.IsNullOrEmpty(Ccnumber) || string.IsNullOrEmpty(Ccexpiry))
                {
                    TempData["ErrorMessage"] = "All fields are required.";
                    return RedirectToAction("Index", new { customerId });
                }

                // Parse expiry date (assuming MM/YY format)
                if (!DateTime.TryParseExact(Ccexpiry, "MM/yy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime expiryDate))
                {
                    TempData["ErrorMessage"] = "Invalid expiry date format.";
                    return RedirectToAction("Index", new { customerId });
                }

                // Remove spaces from card number
                Ccnumber = Ccnumber.Replace(" ", "");

                var creditCard = new CreditCard
                {
                    CustomerId = customerId,
                    Ccname = Ccname,
                    Ccnumber = Ccnumber,
                    Ccexpiry = expiryDate
                };

                var result = await _walletService.AddCreditCardAsync(creditCard);

                if (result)
                {
                    TempData["SuccessMessage"] = "Credit card added successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add credit card.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding credit card for CustomerId: {CustomerId}", customerId);
                TempData["ErrorMessage"] = "An error occurred while adding the credit card.";
            }

            return RedirectToAction("Index", new { customerId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCreditCard(int customerId, int creditCardId)
        {
            try
            {
                var result = await _walletService.DeleteCreditCardAsync(customerId, creditCardId);

                if (result)
                {
                    TempData["SuccessMessage"] = "Credit card deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete credit card.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting credit card. CustomerId: {CustomerId}, CreditCardId: {CreditCardId}",
                    customerId, creditCardId);
                TempData["ErrorMessage"] = "An error occurred while deleting the credit card.";
            }

            return RedirectToAction("Index", new { customerId });
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

