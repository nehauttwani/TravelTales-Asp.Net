using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Travel_Agency___Data;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly TravelExpertsContext _context;
        private readonly BookingManager bookingManager;
        private readonly PackageManager packageManager;
        private readonly CustomerManager customerManager;
        private readonly UserManager<User> userManager;
        private readonly EmailService _emailService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            TravelExpertsContext context,
            UserManager<User> userManager,
            EmailService emailService,
            ILogger<BookingController> logger)
        {
            _context = context;
            bookingManager = new BookingManager(_context);
            packageManager = new PackageManager(_context);
            customerManager = new CustomerManager(_context);
            this.userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Book(int id)
        {
            var package = packageManager.GetPackage(id);
            if (package == null)
            {
                return NotFound();
            }

            var viewModel = new BookingViewModel
            {
                PackageId = package.PackageId,
                PackageName = package.PkgName,
                PackageImage = package.ImagePath,
                TripStart = package.PkgStartDate ?? DateTime.Now,
                TripEnd = package.PkgEndDate ?? DateTime.Now,
                Price = package.PkgBasePrice,
                Description = package.PkgDesc,
                AgencyCommission = package.PkgAgencyCommission ?? 0,
                TripTypes = _context.TripTypes.ToList(),
                Classes = _context.Classes.ToList(),
            };

            return View(viewModel);
        }


        //testing 

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SendTestEmail()
        {
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Index", "Home");
                }

                var testBooking = new BookingConfirmationModel
                {
                    BookingNo = "TEST-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CustomerName = user.FullName ?? "Test Customer",
                    PackageName = "Test Package",
                    TripStart = DateTime.Now.AddDays(7),
                    TripEnd = DateTime.Now.AddDays(14),
                    TravelerCount = 2,
                    TotalPrice = 1999.99m
                };

                await _emailService.SendBookingConfirmationEmailAsync(user.Email, testBooking);
                TempData["SuccessMessage"] = $"Test email sent successfully to {user.Email}";
                _logger.LogInformation($"Test email sent to {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email");
                TempData["ErrorMessage"] = "Failed to send test email: " + ex.Message;
            }

            return RedirectToAction("Profile", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Book(BookingViewModel viewModel)
        {
            try
            {
                viewModel.TripTypes = _context.TripTypes.ToList();
                viewModel.Classes = _context.Classes.ToList();
                ModelState.Remove("BookingNo");

                if (ModelState.IsValid)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await userManager.FindByIdAsync(userId!);

                    if (user != null && user.CustomerId.HasValue)
                    {
                        viewModel.CustomerId = user.CustomerId.Value;
                        var customer = await customerManager.GetCustomerAsync(user.CustomerId.Value);
                        viewModel.BookingNo = customer != null
                            ? GenerateBookingNumber(customer.CustFirstName)
                            : GenerateBookingNumber("Guest");

                        var booking = new Booking
                        {
                            BookingDate = viewModel.BookingDate,
                            BookingNo = viewModel.BookingNo,
                            TravelerCount = viewModel.TravelerCount,
                            CustomerId = viewModel.CustomerId,
                            TripTypeId = viewModel.TripTypeId,
                            PackageId = viewModel.PackageId
                        };

                        bookingManager.AddBooking(booking);

                        var defaultProductSupplierId = _context.ProductsSuppliers.First().ProductSupplierId;

                        var bookingDetail = new BookingDetail
                        {
                            BookingId = booking.BookingId,
                            ItineraryNo = viewModel.CustomerId,
                            TripStart = viewModel.TripStart,
                            TripEnd = viewModel.TripEnd,
                            Description = viewModel.Description,
                            Destination = viewModel.Destination,
                            BasePrice = viewModel.Price * viewModel.TravelerCount,
                            AgencyCommission = viewModel.AgencyCommission,
                            ClassId = viewModel.ClassId,
                            ProductSupplierId = defaultProductSupplierId
                        };

                        bookingManager.AddBookingDetails(bookingDetail);

                        // Get package details for email
                        var package = packageManager.GetPackage(viewModel.PackageId);

                        // Create confirmation model
                        var confirmationModel = new BookingConfirmationModel
                        {
                            BookingNo = booking.BookingNo,
                            CustomerName = $"{customer.CustFirstName} {customer.CustLastName}",
                            PackageName = package.PkgName,
                            TripStart = viewModel.TripStart,
                            TripEnd = viewModel.TripEnd,
                            TravelerCount = viewModel.TravelerCount,
                            TotalPrice = bookingDetail.BasePrice ?? 0m
                        };

                        // Send confirmation email
                        await _emailService.SendBookingConfirmationEmailAsync(user.Email, confirmationModel);

                        TempData["SuccessMessage"] = "Booking confirmed! A confirmation email has been sent to your email address.";

                        return RedirectToAction("Purchase", "Purchase", new
                        {
                            packageId = viewModel.PackageId,
                            customerId = viewModel.CustomerId,
                            travelerCount = viewModel.TravelerCount,
                            totalPrice = bookingDetail.BasePrice
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Customer information not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing booking");
                ModelState.AddModelError("", "An error occurred while processing your booking.");
            }

            return View(viewModel);
        }

        private string GenerateBookingNumber(string firstName)
        {
            return "TT01-" + firstName + "-" + DateTime.Now.ToString("yyyyMMdd");
        }
    }
}