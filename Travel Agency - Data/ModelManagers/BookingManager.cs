using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ModelManagers
{
    public class BookingManager
    {
        private TravelExpertsContext _context { get; set; }

        public BookingManager(TravelExpertsContext ctx)
        {
            _context = ctx;
        }

        public void AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();

        }

        public void AddBookingDetails(BookingDetail bookingDetail)
        {
            _context.BookingDetails.Add(bookingDetail);
            _context.SaveChanges();

        }

        public Booking? GetBookingInfo(int Id)
        {
            var booking = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Package)
                .FirstOrDefault(b => b.BookingId == Id);
            if (booking != null) return booking;
            else return null;
        }

        public BookingDetail? GetBookingDetails(int bookingId)
        {
            var bookingDetail = _context.BookingDetails
                .FirstOrDefault(b => b.BookingId == bookingId);
            if (bookingDetail != null) return bookingDetail;
            else return null;
        }
    }
}
