using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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


        public BookingController(TravelExpertsContext context)
        {
            _context = context;
            bookingManager = new BookingManager(_context);
            packageManager = new PackageManager(_context);
            customerManager = new CustomerManager(_context);
        }

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
                //PackageImage= package.ImagePath,
                TripStart = package.PkgStartDate ?? DateTime.Now,
                TripEnd = package.PkgEndDate ?? DateTime.Now,
                Price = package.PkgBasePrice,
                Description = package.PkgDesc,
                AgencyCommission = package.PkgAgencyCommission ?? 0,
                TripTypes = _context.TripTypes.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookingViewModel viewModel)
        {
            viewModel.TripTypes = _context.TripTypes.ToList();
            ModelState.Remove("BookingNo");
            if (ModelState.IsValid)
            {
                var customer = customerManager.GetCustomer(viewModel.CustomerId);
                if (customer != null)
                {
                    viewModel.BookingNo = GenerateBookingNumber(customer.CustFirstName);
                }
                else
                {
                    viewModel.BookingNo = GenerateBookingNumber("Guest");
                }
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

                var bookingDetail = new BookingDetail
                {
                    BookingId = booking.BookingId,
                    ItineraryNo = 1,
                    TripStart = viewModel.TripStart,
                    TripEnd = viewModel.TripEnd,
                    Description = viewModel.Description,
                    Destination = viewModel.Destination,
                    BasePrice = viewModel.Price*viewModel.TravelerCount,
                    AgencyCommission = viewModel.AgencyCommission
                };

                bookingManager.AddBookingDetails(bookingDetail);

                return RedirectToAction("Confirmation", new { id = booking.BookingId });
            }

            viewModel.TripTypes = _context.TripTypes.ToList();
            return View(viewModel);
        }

        public IActionResult Confirmation(int id)
        {
            var booking = bookingManager.GetBookingInfo(id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        private string GenerateBookingNumber(string firstName)
        {
            return "TE01-" + firstName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
