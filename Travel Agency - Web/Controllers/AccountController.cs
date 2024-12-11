using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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

        // GET: AccountController/Register
        public ActionResult Register()
        {
            var registerViewModel = new RegisterViewModel()
            {
                Agents = _agentsAndAgenciesManager.GetAgents()
            };
            return View(registerViewModel);
        }

        // POST: AccountController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                // Create the user object for Identity
                User user = new User()
                {
                    UserName = registerViewModel.CustEmail,
                    Email = registerViewModel.CustEmail,
                    FullName = $"{registerViewModel.CustFirstName} {registerViewModel.CustLastName}",
                    PhoneNumber = registerViewModel.CustBusPhone
                };

                // Create the user in Identity
                var result = await userManager.CreateAsync(user, registerViewModel.Password!);

                if (result.Succeeded)
                {
                    // Create the Customer object
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

                    // Add customer asynchronously
                    await _customerManager.AddCustomerAsync(customer);

                    // Link the customer to the user
                    user.CustomerId = customer.CustomerId;

                    // Update the user with the CustomerId
                    await userManager.UpdateAsync(user);

                    // Sign the user in
                    await signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to the home page after successful registration
                    return RedirectToAction("Index", "Home");
                }

                // If registration fails, add errors to ModelState
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            // Return the registration view with any errors
            return View(registerViewModel);
        }

        // GET: AccountController/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: AccountController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(LoginViewModal loginViewModal)
        {
            if (ModelState.IsValid) // Check if model is valid
            {
                var result = await signInManager.PasswordSignInAsync(loginViewModal.Username!, loginViewModal.Password!, loginViewModal.RememberMe, false);
                if (result.Succeeded) // If successful, go to home page
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

        // GET: AccountController/Logout
        public async Task<ActionResult> Logout()
        {
            // Sign out the user
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: AccountController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: AccountController/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
