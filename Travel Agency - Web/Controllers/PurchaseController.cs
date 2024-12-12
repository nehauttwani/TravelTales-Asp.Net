using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Travel_Agency___Data;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Web.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly WalletService _walletService;
        private readonly PurchaseService _purchaseService;
        private readonly TravelExpertsContext _context;

        public PurchaseController(WalletService walletService, PurchaseService purchaseService, TravelExpertsContext context)
        {
            _walletService = walletService;
            _purchaseService = purchaseService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> PurchasedProducts(int customerId)
        {
            var products = await _context.Purchases
                .Where(p => p.CustomerId == customerId)
                .Select(p => new PurchasedProductViewModel
                {
                    ProductName = p.ProductName,
                    BasePrice = p.BasePrice,
                    Tax = p.Tax,
                    TotalPrice = p.TotalPrice,
                    PurchaseDate = p.PurchaseDate
                })
                .ToListAsync();

            var totalPaid = products.Sum(p => p.TotalPrice);

            var viewModel = new PurchasedProductsSummaryViewModel
            {
                Products = products,
                TotalPaid = totalPaid
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Purchase(int packageId, int customerId, int travelerCount, decimal totalPrice)
        {
            var package = _context.Packages.FirstOrDefault(p => p.PackageId == packageId);
            if (package == null)
            {
                return NotFound("Package not found.");
            }

            var walletBalance = _walletService.GetWalletBalanceAsync(customerId).Result;

            var viewModel = new PurchaseViewModel
            {
                PackageId = package.PackageId,
                CustomerId = customerId,
                PackageName = package.PkgName,
                Description = package.PkgDesc,
                PricePerPerson = package.PkgBasePrice,
                TravelerCount = travelerCount,
                TotalPrice = totalPrice,
                WalletBalance = walletBalance
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPurchase(int customerId, int packageId, decimal totalPrice)
        {
            var package = await _purchaseService.GetPackageAsync(packageId);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Package not found.";
                return RedirectToAction("Purchase", new { packageId, customerId, travelerCount = 1 });
            }

            var tax = totalPrice * 0.05m;
            var finalPrice = totalPrice + tax;

            var isPaymentSuccessful = await _walletService.DeductFundsAsync(customerId, finalPrice);
            if (!isPaymentSuccessful)
            {
                TempData["ErrorMessage"] = "Payment failed. Please ensure you have sufficient funds in your wallet.";
                return RedirectToAction("Purchase", new { packageId, customerId, travelerCount = 1 });
            }

            var purchase = new Purchase
            {
                CustomerId = customerId,
                PackageId = packageId,
                ProductName = package.PkgName,
                Tax = tax,
                BasePrice = package.PkgBasePrice,
                TotalPrice = finalPrice,
                Price = totalPrice,
                PurchaseDate = DateTime.Now,
                IsPaid = true
            };

            await _purchaseService.SavePurchaseAsync(purchase);

            var confirmation = new PurchaseConfirmationViewModel
            {
                PackageName = package.PkgName,
                BasePrice = totalPrice,
                Tax = tax,
                TotalPrice = finalPrice
            };
            TempData["ConfirmationDetails"] = JsonConvert.SerializeObject(confirmation);

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
