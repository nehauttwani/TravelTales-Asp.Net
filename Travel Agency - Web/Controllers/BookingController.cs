using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
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

        // Constructor
        public BookingController(TravelExpertsContext context)
        {
            _context = context;
            bookingManager = new BookingManager(_context);
            packageManager = new PackageManager(_context);
            customerManager = new CustomerManager(_context);
        }

        // GET: Booking
        [HttpGet]
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
                TripStart = package.PkgStartDate ?? DateTime.Now,  // Handle nullable DateTime
                TripEnd = package.PkgEndDate ?? DateTime.Now,      // Handle nullable DateTime
                Price = package.PkgBasePrice,
                Description = package.PkgDesc,
                AgencyCommission = package.PkgAgencyCommission ?? 0,  // Handle nullable decimal
                TripTypes = _context.TripTypes.ToList()
            };

            return View(viewModel);
        }

        // POST: Booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookingViewModel viewModel)
        {
            // Load TripTypes for the view
            viewModel.TripTypes = _context.TripTypes.ToList();
            ModelState.Remove("BookingNo");

            if (ModelState.IsValid)
            {
                // Ensure TravelerCount is a valid non-nullable int
                int travelerCount = viewModel.TravelerCount;  // TravelerCount is guaranteed to be non-nullable now

                // Fetch customer information
                var customer = await customerManager.GetCustomerAsync((int)viewModel.CustomerId);

                // Generate BookingNo based on customer info
                if (customer != null)
                {
                    viewModel.BookingNo = GenerateBookingNumber(customer.CustFirstName);
                }
                else
                {
                    viewModel.BookingNo = GenerateBookingNumber("Guest");
                }

                // Create a new Booking object
                var booking = new Booking
                {
                    BookingDate = viewModel.BookingDate,
                    BookingNo = viewModel.BookingNo,
                    TravelerCount = travelerCount,  // TravelerCount is now guaranteed to be a valid int
                    CustomerId = viewModel.CustomerId,
                    TripTypeId = viewModel.TripTypeId,
                    PackageId = viewModel.PackageId
                };

                bookingManager.AddBooking(booking);

                // Create a BookingDetail object
                var bookingDetail = new BookingDetail
                {
                    BookingId = booking.BookingId,
                    ItineraryNo = 1,
                    TripStart = viewModel.TripStart,
                    TripEnd = viewModel.TripEnd,
                    Description = viewModel.Description,
                    Destination = viewModel.Destination,
                    BasePrice = viewModel.Price * travelerCount,  // Correctly use travelerCount
                    AgencyCommission = viewModel.AgencyCommission
                };

                bookingManager.AddBookingDetails(bookingDetail);

                // Redirect to confirmation
                return RedirectToAction("Confirmation", new { id = booking.BookingId });
            }

            // Reload TripTypes in case of validation errors
            viewModel.TripTypes = _context.TripTypes.ToList();
            return View(viewModel);
        }

        // Confirmation Page
        public IActionResult Confirmation(int id)
        {
            var booking = bookingManager.GetBookingInfo(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // Generate a unique booking number
        private string GenerateBookingNumber(string firstName)
        {
            return "TE01-" + firstName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
