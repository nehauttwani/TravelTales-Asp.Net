// BookingController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Travel_Agency___Data;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data.Models;
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

        public BookingController(TravelExpertsContext context, UserManager<User> userManager)
        {
            _context = context;
            bookingManager = new BookingManager(_context);
            packageManager = new PackageManager(_context);
            customerManager = new CustomerManager(_context);
            this.userManager = userManager;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Book(BookingViewModel viewModel)
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

                    // Get a default ProductSupplierId from the database
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
                        ProductSupplierId = defaultProductSupplierId  // Set a default value
                    };

                    bookingManager.AddBookingDetails(bookingDetail);

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

            return View(viewModel);
        }

        private string GenerateBookingNumber(string firstName)
        {
            return "TT01-" + firstName + "-" + DateTime.Now.ToString("yyyyMMdd");
        }
    }
}