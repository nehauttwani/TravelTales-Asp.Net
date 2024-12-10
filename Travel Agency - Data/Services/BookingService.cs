using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.Services
{
    public class BookingService
    {
        private readonly TravelExpertsContext _context;

        public BookingService(TravelExpertsContext context)
        {
            _context = context;
        }

        // Retrieve a list of bookings for a specific customer
        public async Task<List<Booking>> GetCustomerBookingsAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .Include(b => b.Package)
                .ToListAsync();
        }

        // Add a new booking
        public async Task AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }
    }
}
