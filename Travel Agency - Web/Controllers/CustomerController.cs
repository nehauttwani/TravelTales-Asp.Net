using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data;
using Travel_Agency___Data.Business;
using Travel_Agency___Data.ViewModels;
using Travel_Agency___Data.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Travel_Agency___Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TravelExpertsContext _context;

        public CustomerController(TravelExpertsContext context)
        {
            _context = context;
        }

        // Search Page
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(string customerNameOrId)
        {
            if (string.IsNullOrEmpty(customerNameOrId))
            {
                TempData["ErrorMessage"] = "Please enter a valid customer ID or name.";
                return RedirectToAction(nameof(Search));
            }

            // searching by ID (integer)
            if (int.TryParse(customerNameOrId, out int customerId))
            {
                var customer = CustomerManager.GetCustomerById(_context, customerId);
                if (customer != null)
                {
                    return RedirectToAction(nameof(Details), new { customerId });
                }
            }

            //// searching by ID (integer)
            //if (int.TryParse(customerNameOrId, out int customerId))
            //{
            //    var customer = CustomerManager.GetCustomerById(_context, customerId);
            //    if (customer != null)
            //    {
            //        return RedirectToAction(nameof(Details), new { customerId });
            //    }
            //}

            // Search by name (string)
            var matchedCustomers = CustomerManager.GetCustomersByName(_context, customerNameOrId);
            if (matchedCustomers.Count == 1)
            {
                return RedirectToAction(nameof(Details), new { customerId = matchedCustomers.First().CustomerId });
            }

            TempData["ErrorMessage"] = "No customer found with the provided ID or name.";
            return RedirectToAction(nameof(Search));
        }

        // Details Page for a Specific Customer
        public IActionResult Details(int customerId)
        {
            var customer = CustomerManager.GetCustomerById(_context, customerId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction(nameof(Search));
            }

            ViewBag.Customer = customer;

            var purchases = CustomerManager.GetCustomerPurchases(_context, customerId); // Fetches data from the database
            return View(purchases); // Passes the data to the Purchases.cshtml view;
        }


        //public IActionResult Index()
        //{
        //    var customers = CustomerManager.GetCustomers(_context);
        //    return View(customers);
        //}

        //public IActionResult Details(int id)
        //{
        //    var customer = CustomerManager.GetCustomerById(_context, id);
        //    if (customer == null)
        //    {
        //        TempData["Error"] = "Customer not found.";
        //        return RedirectToAction("Index");
        //    }
        //    return View(customer);
        //}

        //public IActionResult Purchases(int customerId)
        //{
        //    try
        //    {
        //        // Fetching the purchases for the customer
        //        var purchases = CustomerManager.GetCustomerPurchases(_context, customerId);

        //        if (purchases == null || !purchases.Any())
        //        {
        //            TempData["Message"] = "No purchases found for this customer.";
        //            return RedirectToAction("Index");
        //        }

        //        return View(purchases); // Pass the purchases to the Purchases.cshtml view
        //    }

        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = $"An error occurred: {ex.Message}";
        //        return RedirectToAction("Index");
        //    }

        //}
    }
}

