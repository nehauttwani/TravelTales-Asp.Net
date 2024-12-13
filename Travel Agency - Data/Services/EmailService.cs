// Travel_Agency___Data/Services/EmailService.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Data.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendBookingConfirmationEmailAsync(string recipientEmail, BookingConfirmationModel booking)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["EmailSettings:Port"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var password = _configuration["EmailSettings:SenderPassword"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Travel Tales", senderEmail));
                message.To.Add(new MailboxAddress("", recipientEmail));
                message.Subject = $"Travel Tales - Booking Confirmation #{booking.BookingNo}";

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h2 style='color: #2c3e50;'>Booking Confirmation</h2>
                        <p>Dear {booking.CustomerName},</p>
                        <p>Thank you for choosing Travel Tales! Your booking has been confirmed.</p>
                        
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='color: #2c3e50;'>Booking Details:</h3>
                            <ul style='list-style: none; padding: 0;'>
                                <li><strong>Booking Number:</strong> {booking.BookingNo}</li>
                                <li><strong>Package:</strong> {booking.PackageName}</li>
                                <li><strong>Trip Start:</strong> {booking.TripStart:MMMM dd, yyyy}</li>
                                <li><strong>Trip End:</strong> {booking.TripEnd:MMMM dd, yyyy}</li>
                                <li><strong>Number of Travelers:</strong> {booking.TravelerCount}</li>
                                <li><strong>Total Amount:</strong> ${booking.TotalPrice:N2}</li>
                            </ul>
                        </div>

                        <p>If you have any questions about your booking, please don't hesitate to contact us.</p>
                        
                        <div style='margin-top: 30px;'>
                            <p>Best regards,<br>
                            <strong>Travel Tales Team</strong></p>
                        </div>
                        
                        <div style='margin-top: 30px; font-size: 12px; color: #666;'>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>";

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Booking confirmation email sent successfully to {recipientEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send booking confirmation email to {recipientEmail}");
                throw;
            }
        }
    }
}