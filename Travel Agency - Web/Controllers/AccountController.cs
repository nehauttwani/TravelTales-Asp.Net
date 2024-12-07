using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data;
using Travel_Agency___Data.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Travel_Agency___Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly TravelExpertsContext _context;

        // Constructor to initialize the context
        public AccountController(TravelExpertsContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Check if the customer exists in the database with the provided email and password
                var user = _context.Customers
                    .FirstOrDefault(c => c.CustEmail == customer.CustEmail && c.CustPassword == HashPassword(customer.CustPassword));

                if (user != null)
                {
                    // If the user is found, store the user's ID in a session or cookie
                    HttpContext.Session.SetInt32("CustomerId", user.CustomerId);
                    return RedirectToAction("Index", "Home"); // Redirect to home page or dashboard
                }
                else
                {
                    // If no match is found, return an error message
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            return View(customer); // Return the view if the login failed or the model is invalid
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingCustomer = _context.Customers
                    .FirstOrDefault(c => c.CustEmail == customer.CustEmail);

                if (existingCustomer == null)
                {
                    // Hash the password before saving
                    customer.CustPassword = HashPassword(customer.CustPassword);

                    // Add the new customer to the database
                    _context.Customers.Add(customer);
                    _context.SaveChanges();

                    // Optionally log in the user after registration
                    HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);

                    return RedirectToAction("Index", "Home"); // Redirect to the home page after successful registration
                }
                else
                {
                    ModelState.AddModelError("", "This email is already registered.");
                }
            }

            return View(customer); // Return the view if the registration failed or the model is invalid
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            // Remove the customer session
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Login"); // Redirect to the login page
        }

        // Helper method to hash passwords securely using SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
