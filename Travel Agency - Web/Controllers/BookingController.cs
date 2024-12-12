using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
        private readonly IConfiguration _configuration;


        public BookingController(TravelExpertsContext context, IConfiguration configuration)
        {
            _context = context;
            bookingManager = new BookingManager(_context);
            packageManager = new PackageManager(_context);
            customerManager = new CustomerManager(_context);
            _configuration = configuration; // Inject configuration to access appsettings.json
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
                PackageImage= package.ImagePath,
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

            // Send a confirmation email to the customer
            SendConfirmationEmail(booking);
            return View(booking);
        }

        private void SendConfirmationEmail(Booking booking)
        {
            try
            {
                // Get email settings from configuration
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["EmailSettings:Port"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];

                using (var smtpClient = new SmtpClient(smtpServer, port))
                {
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;

                    if (booking.CustomerId.HasValue)
                    {
                        var customerEmail = customerManager.GetCustomer(booking.CustomerId.Value)?.CustEmail;
                        if (string.IsNullOrEmpty(customerEmail))
                        {
                            Console.WriteLine("Customer email not found. Skipping email notification.");
                            return;
                        }

                        var subject = "Booking Confirmation - Travel Agency";
                        var body = $@"
                    Dear {booking.Customer?.CustFirstName} {booking.Customer?.CustLastName},

                    Thank you for your booking! Here are your booking details:
                    - Booking Number: {booking.BookingNo}
                    - Package: {booking.Package?.PkgName}
                    - Booking Date: {booking.BookingDate:MM/dd/yyyy}
                    - Traveler Count: {booking.TravelerCount}

                    We look forward to serving you.

                    Best regards,
                    Travel Agency Team
                ";

                        var mailMessage = new MailMessage(senderEmail, customerEmail, subject, body);
                        smtpClient.Send(mailMessage);

                        Console.WriteLine("Booking confirmation email sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine("CustomerId is null. Cannot send email.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        
    }

        private string GenerateBookingNumber(string firstName)
        {
            return "TE01-" + firstName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
