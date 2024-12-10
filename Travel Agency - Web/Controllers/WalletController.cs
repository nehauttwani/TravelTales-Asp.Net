using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.Models;
using System.Threading.Tasks;

namespace Travel_Agency___Web.Controllers
{
    public class WalletController : Controller
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }

        // GET: Wallet Balance
        [HttpGet]
        public async Task<IActionResult> Index(int customerId)
        {
            var balance = await _walletService.GetWalletBalanceAsync(customerId);
            ViewData["WalletBalance"] = balance;
            ViewData["CustomerId"] = customerId;
            return View();
        }

        // POST: Add Funds to Wallet
        [HttpPost]
        public async Task<IActionResult> AddFunds(int customerId, decimal amount)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "Amount should be greater than zero.");
                return RedirectToAction("Index", new { customerId });
            }

            var success = await _walletService.AddFundsAsync(customerId, amount);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to add funds. Please try again.");
            }

            return RedirectToAction("Index", new { customerId });
        }

        // POST: Deduct Funds from Wallet
        [HttpPost]
        public async Task<IActionResult> DeductFunds(int customerId, decimal amount)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "Amount should be greater than zero.");
                return RedirectToAction("Index", new { customerId });
            }

            var success = await _walletService.DeductFundsAsync(customerId, amount);
            if (!success)
            {
                ModelState.AddModelError("", "Insufficient funds or transaction error.");
            }

            return RedirectToAction("Index", new { customerId });
        }
    }
}
