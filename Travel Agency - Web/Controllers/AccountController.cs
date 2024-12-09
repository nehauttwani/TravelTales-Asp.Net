using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Travel_Agency___Data;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ViewModels;

namespace Travel_Agency___Web.Controllers
{
    public class AccountController : Controller
    {
        private TravelExpertsContext _context { get; set; }
        private CustomerManager _customerManager;
        private AgentsAndAgenciesManager _agentsAndAgenciesManager;

        //Identity object to manage signin
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public AccountController(TravelExpertsContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _customerManager = new CustomerManager(_context);
            _agentsAndAgenciesManager = new AgentsAndAgenciesManager(_context);
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        // GET: AccountController
        public ActionResult Register()
        {
            var registerViewModel = new RegisterViewModel()
            {
                Agents = _agentsAndAgenciesManager.GetAgents()
            };
            return View(registerViewModel);
        }

        // Constructor to initialize the context
        public AccountController(TravelExpertsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(LoginViewModal loginViewModal)
        {
            if (ModelState.IsValid) //Check if model is valid
            {
                var result = await signInManager.PasswordSignInAsync(loginViewModal.Username!, loginViewModal.Password!, loginViewModal.RememberMe, false);
                if (result.Succeeded)//if successful, go to home page.
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login");
                    return View();
                }
            }
            return View();
        }

        public async Task<ActionResult> Logout()
        {
            //sign out 
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: AccountController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        

        // POST: AccountController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    UserName = registerViewModel.CustEmail,
                    Email = registerViewModel.CustEmail,
                    FullName = $"{registerViewModel.CustFirstName} {registerViewModel.CustLastName}",
                    PhoneNumber = registerViewModel.CustBusPhone
                };

                var result = await userManager.CreateAsync(user, registerViewModel.Password!);

                if (result.Succeeded)
                {
                    var customer = new Customer
                    {
                        CustFirstName = registerViewModel.CustFirstName!,
                        CustLastName = registerViewModel.CustLastName!,
                        CustAddress = registerViewModel.CustAddress!,
                        CustCity = registerViewModel.CustCity!,
                        CustProv = registerViewModel.CustProv!,
                        CustPostal = registerViewModel.CustPostal!,
                        CustCountry = registerViewModel.CustCountry!,
                        CustHomePhone = registerViewModel.CustHomePhone!,
                        CustBusPhone = registerViewModel.CustBusPhone!,
                        CustEmail = registerViewModel.CustEmail!
                    };

                    _customerManager.AddCustomer(customer);
                    await _context.SaveChangesAsync();

                    user.CustomerId = customer.CustomerId;
                    await userManager.UpdateAsync(user);

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(registerViewModel);
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
