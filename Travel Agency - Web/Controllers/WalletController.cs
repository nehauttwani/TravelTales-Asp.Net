using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;

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
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> AddCreditCard([FromForm] WalletViewModel model)
    {
        try
        {
            // Check if customer already has a credit card
            var existingCard = await _walletService.GetCreditCardsAsync(model.CustomerId);
            if (existingCard?.Any() == true)
            {
                TempData["ErrorMessage"] = "You already have a credit card registered.";
                return RedirectToAction("Index", new { customerId = model.CustomerId });
            }

            // Clean and validate card number
            string cleanCardNumber = model.Ccnumber.Replace("-", "").Replace(" ", "");
            if (!IsValidCardNumber(cleanCardNumber))
            {
                TempData["ErrorMessage"] = "Please enter a valid 16-digit card number.";
                return RedirectToAction("Index", new { customerId = model.CustomerId });
            }

            // Parse and validate expiry date
            string[] dateParts = model.Ccexpiry.Split('/');
            if (dateParts.Length != 2 ||
                !int.TryParse(dateParts[0], out int month) ||
                !int.TryParse(dateParts[1], out int year))
            {
                TempData["ErrorMessage"] = "Invalid expiry date format. Please use MM/YY format.";
                return RedirectToAction("Index", new { customerId = model.CustomerId });
            }

            // Convert to full year (20xx)
            int fullYear = 2000 + year;

            // Create expiry date as last day of the month
            DateTime expiryDate = new DateTime(fullYear, month, 1)
                .AddMonths(1)
                .AddDays(-1);

            // Validate date is in the future
            if (expiryDate < DateTime.Today)
            {
                TempData["ErrorMessage"] = "Card has expired. Please use a valid expiry date.";
                return RedirectToAction("Index", new { customerId = model.CustomerId });
            }

            // Validate date is not too far in the future
            if (expiryDate > DateTime.Today.AddYears(5))
            {
                var maxDate = DateTime.Today.AddYears(5);
                var maxMonth = maxDate.ToString("MM");
                var maxYear = maxDate.ToString("yy");
                TempData["ErrorMessage"] = $"Invalid expiry date. Credit cards usually expire between three and five years after they are issued";
                return RedirectToAction("Index", new { customerId = model.CustomerId });
            }

            var creditCard = new CreditCard
            {
                CustomerId = model.CustomerId,
                Ccname = model.Ccname,
                Ccnumber = cleanCardNumber,
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
            _logger.LogError(ex, "Error adding credit card for CustomerId: {CustomerId}", model.CustomerId);
            TempData["ErrorMessage"] = "An error occurred while adding the credit card.";
        }

        return RedirectToAction("Index", new { customerId = model.CustomerId });
    }
    private bool IsValidCardNumber(string cardNumber)
    {
        // Remove any dashes and spaces
        cardNumber = cardNumber.Replace("-", "").Replace(" ", "");

        // Check if it's exactly 16 digits
        if (cardNumber.Length != 16)
            return false;

        // Check if all characters are digits
        return cardNumber.All(char.IsDigit);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCreditCard(int customerId, int creditCardId)
    {
        if (customerId <= 0 || creditCardId <= 0)
        {
            TempData["ErrorMessage"] = "Invalid request parameters.";
            return RedirectToAction("Index", new { customerId });
        }

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


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFundsWithCreditCard(int customerId, int creditCardId, string amount)
    {
        try
        {
            _logger.LogInformation($"Attempting to add funds. CustomerId: {customerId}, Amount: {amount}");

            // Parse and validate amount
            if (!decimal.TryParse(amount, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal parsedAmount))
            {
                TempData["ErrorMessage"] = "Please enter a valid amount";
                return RedirectToAction("Index", new { customerId });
            }

            // Round to 2 decimal places
            parsedAmount = Math.Round(parsedAmount, 2);

            if (parsedAmount < 10.00m)
            {
                TempData["ErrorMessage"] = "Minimum amount to add is $10.00";
                return RedirectToAction("Index", new { customerId });
            }

            if (parsedAmount > 10000.00m)
            {
                TempData["ErrorMessage"] = "Maximum amount to add is $10,000.00";
                return RedirectToAction("Index", new { customerId });
            }

            var result = await _walletService.AddFundsAsync(customerId, creditCardId, parsedAmount);

            if (result)
            {
                TempData["SuccessMessage"] = $"Successfully added ${parsedAmount:F2} to your wallet";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add funds. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding funds for CustomerId: {CustomerId}", customerId);
            TempData["ErrorMessage"] = "An error occurred while adding funds.";
        }

        return RedirectToAction("Index", new { customerId });
    }

}