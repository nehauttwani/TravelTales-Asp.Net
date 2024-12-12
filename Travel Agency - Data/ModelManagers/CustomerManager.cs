using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;

namespace Travel_Agency___Data.ModelManagers
{
    public class CustomerManager
    {
        private TravelExpertsContext _context { get; set; }

        public CustomerManager(TravelExpertsContext ctx)
        {
            _context = ctx;
        }

        // Asynchronously add a customer
        public async Task AddCustomerAsync(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();  // Use SaveChangesAsync for async operation
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or rethrow as necessary)
                throw new InvalidOperationException("An error occurred while adding the customer.", ex);
            }
        }

        // Asynchronously update an existing customer
        public async Task UpdateCustomerAsync(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();  // Use SaveChangesAsync for async operation
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or rethrow as necessary)
                throw new InvalidOperationException("An error occurred while updating the customer.", ex);
            }
        }

        public Customer GetCustomer(int id)
        {
            return _context.Customers.FirstOrDefault(x => x.CustomerId == id);
        }
    }
}
