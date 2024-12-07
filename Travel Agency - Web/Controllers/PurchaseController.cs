using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;
namespace Travel_Agency___Web.Controllers
{
    public class PurchaseController : Controller
    {
       
            private readonly WalletService _walletService;

            public PurchaseController(WalletService walletService)
            {
                _walletService = walletService;
            }

            // Purchase Page
            public IActionResult Index(int customerId, int packageId, decimal packagePrice)
            {
                var model = new PurchaseViewModel
                {
                    CustomerId = customerId,
                    PackageId = packageId,
                    PackagePrice = packagePrice,
                };
                return View(model); // Ensure the corresponding view is created in Views/Purchase/Index.cshtml
            }

            // Handle wallet purchase
            [HttpPost]
            public async Task<IActionResult> PurchasePackage(int customerId, int packageId, decimal packagePrice)
            {
                var success = await _walletService.DeductForPurchaseAsync(customerId, packagePrice);

                if (!success)
                {
                    return RedirectToAction("Index", "Wallet", new { customerId, errorMessage = "Insufficient wallet balance. Please add funds to complete the purchase." });
                }

                // Logic to save purchase details in the database goes here (not yet implemented)

                return RedirectToAction("Confirmation", new { customerId, packageId });
            }

            // Confirmation Page
            public IActionResult Confirmation(int customerId, int packageId)
            {
                // You can pass additional details for the confirmation view
                return View();
            }
    }
    
}
