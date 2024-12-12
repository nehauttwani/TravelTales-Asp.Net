using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Travel_Agency___Data;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ViewModels;
using System.Configuration;
using System.Net.Mail;
using System.Net;

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
            _configuration = configuration; // Inject configuration for reading appsettings.json
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

            // Send the confirmation email to the customer
            SendConfirmationEmail(booking);
            return View(booking);
        }

        // Method to send a confirmation email
        private void SendConfirmationEmail(Booking booking)
        {
            try
            {
                // Retrieve email configuration from appsettings.json
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["EmailSettings:Port"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];

                using (var smtpClient = new SmtpClient(smtpServer, port))
                {
                    // Configure SMTP client
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;

                    // Check if CustomerId is not null
                    if (booking.CustomerId.HasValue)
                    {
                        // Retrieve the customer's email using the Customer Manager
                        //var customerEmail = customerManager.GetCustomerAsync(booking.CustomerId.Value);

                        if (string.IsNullOrEmpty(booking.Customer.CustEmail))
                        {
                            // Skip email notification if customer email is not found
                            Console.WriteLine("Customer email not found. Skipping email notification.");
                            return;
                        }

                        // Define the email subject and body
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

                        // Create and send the email
                        var mailMessage = new MailMessage(senderEmail, booking.Customer.CustEmail, subject, body);
                        smtpClient.Send(mailMessage);

                        Console.WriteLine("Booking confirmation email sent successfully.");
                    }
                    else
                    {
                        // Log if CustomerId is null
                        Console.WriteLine("CustomerId is null. Cannot send email.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the email-sending process
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }


        private string GenerateBookingNumber(string firstName)
        {
            return "TE01-" + firstName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
