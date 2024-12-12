using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
                PackageImage= package.ImagePath,
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
        [Authorize] // Ensuring the user is authenticated
        public async Task<IActionResult> Book(BookingViewModel viewModel)
        {
            viewModel.TripTypes = _context.TripTypes.ToList();
            ModelState.Remove("BookingNo");
            if (ModelState.IsValid)
            {
                // Get the current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fetch the user from the database
                var user = await userManager.FindByIdAsync(userId!);

                if (user != null && user.CustomerId.HasValue)
                {
                    viewModel.CustomerId = user.CustomerId.Value;
                    var customer = customerManager.GetCustomer(user.CustomerId.Value);
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
                var customer = await customerManager.GetCustomerAsync(viewModel.CustomerId);
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
                        ItineraryNo = viewModel.CustomerId,
                        TripStart = viewModel.TripStart,
                        TripEnd = viewModel.TripEnd,
                        Description = viewModel.Description,
                        Destination = viewModel.Destination,
                        BasePrice = viewModel.Price * viewModel.TravelerCount,
                        AgencyCommission = viewModel.AgencyCommission,
                        ClassId = viewModel.ClassId,
                        ProductSupplierId = 44
                    };

                    bookingManager.AddBookingDetails(bookingDetail);
                    viewModel.BookingId = booking.BookingId;

                    return RedirectToAction("Confirmation", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", "Customer information not found.");
                }
            }

            viewModel.TripTypes = _context.TripTypes.ToList();
            return View(viewModel);
        }


        public IActionResult Confirmation(BookingViewModel bookingViewModel)
        {

            return View(bookingViewModel);
        }

        private string GenerateBookingNumber(string firstName)
        {
            return "TT01-" + firstName + "-" + DateTime.Now.ToString("yyyyMMdd");
        }

        public IActionResult DownloadSummary(int id)
        {
            var booking = bookingManager.GetBookingInfo(id);
            if (booking == null)
            {
                return NotFound();
            }

            var pdf = GenerateBookingSummaryPdf(booking);
            return File(pdf, "application/pdf", $"Booking_Summary_{booking.BookingNo}.pdf");
        }

        
        private byte[] GenerateBookingSummaryPdf(Booking booking)
        {
            var bookingDetail = bookingManager.GetBookingDetails(booking.BookingId);
            booking.BookingDetails = new List<BookingDetail> { bookingDetail! };
            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfDocument document = new PdfDocument())
                {
                    PdfPage page = document.Pages.Add();

                    // Set the page size to fit the content
                    double margin = 50; // Margin around the content
                    double width = page.Width - 2 * margin;
                    double height = 0; // Initialize height

                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont titleFont = new XFont("Arial", 20);
                    XFont headerFont = new XFont("Arial", 14);
                    XFont normalFont = new XFont("Arial", 12);

                    XRect headerRect = new XRect(0, 0, page.Width, 60);
                    gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(4, 20, 65)), headerRect);

                    // Add logo and company name
                    XImage logo = XImage.FromFile("wwwroot/images/logo.png");
                    gfx.DrawImage(logo, 20, 10, 40, 40);
                    gfx.DrawString("Travel Tales", titleFont, XBrushes.White, 70, 35);

                    // Add title
                    gfx.DrawString("Booking Summary", titleFont, XBrushes.Black, new XRect(0, 80, page.Width, page.Height), XStringFormats.TopCenter);

                    // Add booking details in tabular format
                    int yPosition = 120;
                    DrawTableRow(gfx, headerFont, normalFont, "Booking Number:", booking.BookingNo!, ref yPosition);
                    DrawTableRow(gfx, headerFont, normalFont, "Package Name:", booking.Package!.PkgName, ref yPosition);
                    DrawTableRow(gfx, headerFont, normalFont, "Travel Dates:", $"{booking.BookingDetails.First().TripStart:MMM dd, yyyy} - {booking.BookingDetails.First().TripEnd:MMM dd, yyyy}", ref yPosition);
                    DrawTableRow(gfx, headerFont, normalFont, "Number of Travelers:", booking.TravelerCount.ToString(), ref yPosition);
                    DrawTableRow(gfx, headerFont, normalFont, "Total Price:", $"${booking.BookingDetails.First().BasePrice:F2}", ref yPosition);

                    document.Save(ms);
                }
                return ms.ToArray();
            }
        }

        private void DrawTableRow(XGraphics gfx, XFont headerFont, XFont normalFont, string label, string value, ref int yPosition)
        {
            gfx.DrawRectangle(XBrushes.LightGray, 50, yPosition, 200, 25);
            gfx.DrawRectangle(XBrushes.White, 250, yPosition, 300, 25);
            gfx.DrawString(label, headerFont, XBrushes.Black, new XRect(55, yPosition, 190, 25), XStringFormats.CenterLeft);
            gfx.DrawString(value, normalFont, XBrushes.Black, new XRect(255, yPosition, 290, 25), XStringFormats.CenterLeft);
            yPosition += 30;
        }


    }
}
