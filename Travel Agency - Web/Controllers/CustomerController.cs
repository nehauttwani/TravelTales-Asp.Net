using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;
using Travel_Agency___Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Travel_Agency___Controllers
{
    public class CustomerController : Controller
    {
        private readonly TravelExpertsContext _context;

        public CustomerController(TravelExpertsContext context)
        {
            _context = context;
        }

        // GET: Edit Customer
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the customer by ID, including the related Agent
            var customer = await _context.Customers
                .Include(c => c.Agent)  // Include the associated agent for the customer
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Load the list of agents into ViewBag for the dropdown
            ViewBag.AgentId = new SelectList(_context.Agents, "AgentId", "AgtFirstName", customer.AgentId);

            return View(customer);
        }

        // POST: Edit Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustFirstName,CustLastName,CustEmail,CustAddress,CustCity,CustProv,CustPostal,CustCountry,CustHomePhone,CustBusPhone,AgentId")] Customer customer)
        {
            // Log the incoming customer data
            Console.WriteLine($"Received customer data: {customer.CustomerId}, {customer.CustFirstName} {customer.CustLastName}");

            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Log validation errors
                Console.WriteLine("Model validation failed!");
                return View(customer);
            }

            try
            {
                // Log the update attempt
                Console.WriteLine($"Attempting to update customer {customer.CustomerId}: {customer.CustFirstName} {customer.CustLastName}");

                // Ensure entity state is set to modified before saving
                _context.Entry(customer).State = EntityState.Modified;

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Log the successful update
                Console.WriteLine($"Successfully updated customer {customer.CustomerId}: {customer.CustFirstName} {customer.CustLastName}");

                // Redirect to the customer details page after successful update
                return RedirectToAction(nameof(Details), new { id = customer.CustomerId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    // Log the concurrency exception
                    Console.WriteLine($"Concurrency error while updating customer {customer.CustomerId}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log any general exceptions
                Console.WriteLine($"Error updating customer: {ex.Message}");

                // Return the view with the customer data if something went wrong
                return View(customer);
            }
        }

        // GET: Customer Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Agent)  // Include agent information
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // Helper method to check if a customer exists
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
