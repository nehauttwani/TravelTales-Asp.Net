using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Web.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly WalletService _walletService;
        private readonly PurchaseService _purchaseService;

        public PurchaseController(WalletService walletService, PurchaseService purchaseService)
        {
            _walletService = walletService;
            _purchaseService = purchaseService;
        }

        [HttpGet]
        public async Task<IActionResult> Purchase(int packageId, int customerId, int travelerCount = 1)
        {
            // Fetch package details
            var package = await _purchaseService.GetPackageAsync(packageId);
            if (package == null)
            {
                return NotFound("Package not found.");
            }

            // Fetch wallet balance from the Wallets table
            var walletBalance = await _walletService.GetWalletBalanceAsync(customerId);

            // Calculate total price based on traveler count
            var totalPrice = package.PkgBasePrice * travelerCount;

            // Pass data to the view
            var viewModel = new PurchaseViewModel
            {
                PackageId = package.PackageId,
                CustomerId = customerId,
                PackageName = package.PkgName,
                Description = package.PkgDesc ?? string.Empty,
                PricePerPerson = package.PkgBasePrice,
                WalletBalance = walletBalance,
                TravelerCount = travelerCount,
                TotalPrice = totalPrice
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> ProcessPurchase(PurchaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Purchase", model); // Return to the purchase page if validation fails
            }

            // Check if wallet balance is sufficient
            var walletBalance = await _walletService.GetWalletBalanceAsync(model.CustomerId);
            if (walletBalance < model.TotalPrice)
            {
                ModelState.AddModelError("", "Insufficient wallet balance.");
                return View("Purchase", model);
            }

            // Deduct the amount from the wallet
            var success = await _walletService.DeductFundsAsync(model.CustomerId, model.TotalPrice);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to process payment. Please try again.");
                return View("Purchase", model);
            }

            // Save the purchase to the database
            await _purchaseService.SavePurchaseAsync(new Purchase
            {
                CustomerId = model.CustomerId,
                PackageId = model.PackageId,
                ProductName = model.PackageName,
                BasePrice = model.PricePerPerson,
                Tax = model.TotalPrice * 0.1m, // Example tax calculation
                TotalPrice = model.TotalPrice,
                IsPaid = true,
                PurchaseDate = DateTime.Now
            });

            // Redirect to the confirmation page with the purchase details
            return RedirectToAction("Confirmation", new
            {
                packageName = model.PackageName,
                totalPrice = model.TotalPrice
            });
        }

        [HttpGet]
        public IActionResult Confirmation(string packageName, decimal totalPrice)
        {
            var viewModel = new ConfirmationViewModel
            {
                PackageName = packageName,
                TotalPrice = totalPrice
            };

            return View(viewModel);
        }
    }
}
