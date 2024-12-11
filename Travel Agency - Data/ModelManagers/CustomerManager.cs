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

        // Use asynchronous method to add customer
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

        // Asynchronously retrieve customer by ID
        public async Task<Customer> GetCustomerAsync(int id)
        {
            return await _context.Customers
                                 .FirstOrDefaultAsync(x => x.CustomerId == id);  // Use async method here
        }
    }
}
