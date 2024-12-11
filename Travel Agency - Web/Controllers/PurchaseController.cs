using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public async Task<IActionResult> ProcessPurchase(int customerId, int packageId, decimal totalPrice)
        {
            // Fetch the package details
            var package = await _purchaseService.GetPackageAsync(packageId);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Package not found.";
                return RedirectToAction("Purchase", new { packageId, customerId, travelerCount = 1 });
            }

            // Calculate tax and final total
            var tax = totalPrice * 0.05m; // Assuming 5% tax
            var finalPrice = totalPrice + tax;

            // Deduct funds including tax
            var isPaymentSuccessful = await _walletService.DeductFundsAsync(customerId, finalPrice);
            if (!isPaymentSuccessful)
            {
                TempData["ErrorMessage"] = "Payment failed. Please ensure you have sufficient funds in your wallet.";
                return RedirectToAction("Purchase", new { packageId, customerId, travelerCount = 1 });
            }

            // Create and save the purchase record
            var purchase = new Purchase
            {
                CustomerId = customerId,
                PackageId = packageId,
                ProductName = package.PkgName,
                Tax = tax,
                BasePrice = package.PkgBasePrice,
                TotalPrice = finalPrice,
                Price = totalPrice, // Total without tax
                PurchaseDate = DateTime.Now,
                IsPaid = true
            };

            await _purchaseService.SavePurchaseAsync(purchase);

            // Store confirmation details in TempData
            var confirmation = new PurchaseConfirmationViewModel
            {
                PackageName = package.PkgName,
                BasePrice = totalPrice,
                Tax = tax,
                TotalPrice = finalPrice
            };
            TempData["ConfirmationDetails"] = JsonConvert.SerializeObject(confirmation); // Serialize as JSON

            return RedirectToAction("Confirmation");
        }


        [HttpGet]
        public IActionResult Confirmation()
        {
            if (TempData["ConfirmationDetails"] != null)
            {
                var json = TempData["ConfirmationDetails"].ToString();
                var confirmationDetails = JsonConvert.DeserializeObject<PurchaseConfirmationViewModel>(json);
                return View(confirmationDetails);
            }

            ViewBag.ErrorMessage = "No confirmation details found.";
            return View("Error");
        }
    }
}
