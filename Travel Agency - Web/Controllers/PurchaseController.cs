using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
                PurchaseId = purchase.PurchaseId, // Added this to enable PDF download
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

        [HttpGet]
        public IActionResult DownloadSummary(int id)
        {
            // Retrieve the purchase record by ID
            var purchase = _context.Purchases.FirstOrDefault(p => p.PurchaseId == id);
            if (purchase == null)
            {
                return NotFound("Purchase not found.");
            }

            // Generate the PDF document
            var pdfBytes = GeneratePurchaseSummaryPdf(purchase);

            // Return the PDF file for download
            return File(pdfBytes, "application/pdf", $"Payment_Summary_{purchase.PurchaseId}.pdf");
        }

        private byte[] GeneratePurchaseSummaryPdf(Purchase purchase)
        {
            using (var stream = new MemoryStream())
            {
                // Create a new PDF document
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Define fonts
                var titleFont = new XFont("Arial", 20); // Defaults to Regular
                var regularFont = new XFont("Arial", 12);

                // Draw the title
                gfx.DrawString("Payment Summary", titleFont, XBrushes.Black, new XRect(0, 40, page.Width, 30), XStringFormats.TopCenter);

                // Add the purchase details
                gfx.DrawString($"Package Name: {purchase.ProductName}", regularFont, XBrushes.Black, new XRect(50, 100, page.Width - 100, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Base Price: ${purchase.BasePrice:F2}", regularFont, XBrushes.Black, new XRect(50, 130, page.Width - 100, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Tax (5%): ${purchase.Tax:F2}", regularFont, XBrushes.Black, new XRect(50, 160, page.Width - 100, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Total Price: ${purchase.TotalPrice:F2}", regularFont, XBrushes.Black, new XRect(50, 190, page.Width - 100, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Purchase Date: {purchase.PurchaseDate:MMM dd, yyyy}", regularFont, XBrushes.Black, new XRect(50, 220, page.Width - 100, 20), XStringFormats.TopLeft);

                // Save the document to the memory stream
                document.Save(stream);

                // Return the PDF as a byte array
                return stream.ToArray();
            }
        }
    }
}