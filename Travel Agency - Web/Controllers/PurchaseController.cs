using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Web.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly PurchaseService _purchaseService;
        private readonly WalletService _walletService;

        public PurchaseController(PurchaseService purchaseService, WalletService walletService)
        {
            _purchaseService = purchaseService;
            _walletService = walletService;
        }

        // Action to display purchased products
        public async Task<IActionResult> PurchasedProducts(int customerId, string search = null)
        {
            // Fetch purchased products for the given customer
            var purchasedProducts = await _purchaseService.GetPurchasedProductsAsync(customerId);

            // Apply filtering if a search term is provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                purchasedProducts = purchasedProducts
                    .Where(p => p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Prepare the summary view model
            var summaryViewModel = new PurchasedProductsSummaryViewModel
            {
                Products = purchasedProducts,
                TotalPaid = purchasedProducts.Sum(p => p.TotalPrice),
                OutstandingBalance = await _purchaseService.GetOutstandingBalanceAsync(customerId)
            };

            // Pass customerId to the view for future requests
            ViewData["CustomerId"] = customerId;
            return View(summaryViewModel);
        }

        // Action to display available packages for purchase
        public async Task<IActionResult> Index(int customerId)
        {
            // Example: Fetch available packages for purchase
            var availablePackages = new List<PurchaseViewModel>
            {
                new PurchaseViewModel
                {
                    CustomerId = customerId,
                    PackageId = 1,
                    PackagePrice = 100.00m,
                    WalletBalance = await _walletService.GetWalletBalanceAsync(customerId)
                },
                new PurchaseViewModel
                {
                    CustomerId = customerId,
                    PackageId = 2,
                    PackagePrice = 200.00m,
                    WalletBalance = await _walletService.GetWalletBalanceAsync(customerId)
                }
            };

            return View(availablePackages);
        }

        // Action to handle package purchase
        [HttpPost]
        public async Task<IActionResult> Purchase(int customerId, int packageId, decimal packagePrice)
        {
            var walletBalance = await _walletService.GetWalletBalanceAsync(customerId);

            if (walletBalance < packagePrice)
            {
                return BadRequest("Insufficient wallet balance for this purchase.");
            }

            // Deduct funds from wallet automatically
            var isDeducted = await _walletService.DeductFundsAsync(customerId, packagePrice);

            if (!isDeducted)
            {
                return BadRequest("Failed to deduct funds. Please try again.");
            }

            // Here you can add logic to record the purchase in the database
            // Example: await _purchaseService.RecordPurchase(customerId, packageId, packagePrice);

            // Redirect to the wallet page or a confirmation page
            return RedirectToAction("Index", "Wallet", new { customerId });
        }
    }
}
