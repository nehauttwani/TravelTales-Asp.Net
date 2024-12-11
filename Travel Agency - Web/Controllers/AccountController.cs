using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data.ViewModels;
using System.Threading.Tasks;
using Travel_Agency___Data;

namespace Travel_Agency___Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly TravelExpertsContext _context;
        private readonly CustomerManager _customerManager;
        private readonly AgentsAndAgenciesManager _agentsAndAgenciesManager;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        // Constructor
        public AccountController(
            TravelExpertsContext context,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            CustomerManager customerManager,
            AgentsAndAgenciesManager agentsAndAgenciesManager)
        {
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
            _customerManager = customerManager;
            _agentsAndAgenciesManager = agentsAndAgenciesManager;
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            var registerViewModel = new RegisterViewModel()
            {
                // Ensure that Agents is not null by using null-coalescing operator
                Agents = _agentsAndAgenciesManager.GetAgents() ?? new List<Agent>()
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

        // GET: Account/Profile
        public async Task<ActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var customer = await _customerManager.GetCustomerAsync(user.CustomerId.Value);
            if (customer == null)
            {
                return RedirectToAction("Login");
            }

            var profileViewModel = new ProfileViewModel
            {
                CustomerId = customer.CustomerId,
                CustFirstName = customer.CustFirstName,
                CustLastName = customer.CustLastName,
                CustAddress = customer.CustAddress,
                CustCity = customer.CustCity,
                CustProv = customer.CustProv,
                CustPostal = customer.CustPostal,
                CustCountry = customer.CustCountry,
                CustHomePhone = customer.CustHomePhone,
                CustBusPhone = customer.CustBusPhone,
                CustEmail = customer.CustEmail
            };

            return View(profileViewModel);
        }

        // GET: Account/EditProfile
        public async Task<ActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var customer = await _customerManager.GetCustomerAsync(user.CustomerId.Value);
            if (customer == null)
            {
                return RedirectToAction("Login");
            }

            var profileViewModel = new ProfileViewModel
            {
                CustomerId = customer.CustomerId,
                CustFirstName = customer.CustFirstName,
                CustLastName = customer.CustLastName,
                CustAddress = customer.CustAddress,
                CustCity = customer.CustCity,
                CustProv = customer.CustProv,
                CustPostal = customer.CustPostal,
                CustCountry = customer.CustCountry,
                CustHomePhone = customer.CustHomePhone,
                CustBusPhone = customer.CustBusPhone,
                CustEmail = customer.CustEmail
            };

            return View(profileViewModel);
        }

        // POST: Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User); // Get the current user
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // Update customer information
                var customer = await _customerManager.GetCustomerAsync(user.CustomerId.Value);
                if (customer == null)
                {
                    return RedirectToAction("Login");
                }

                // Update basic customer profile fields
                customer.CustFirstName = model.CustFirstName;
                customer.CustLastName = model.CustLastName;
                customer.CustAddress = model.CustAddress;
                customer.CustCity = model.CustCity;
                customer.CustProv = model.CustProv;
                customer.CustPostal = model.CustPostal;
                customer.CustCountry = model.CustCountry;
                customer.CustHomePhone = model.CustHomePhone;
                customer.CustBusPhone = model.CustBusPhone;
                customer.CustEmail = model.CustEmail;

                // Save changes to customer profile
                await _customerManager.UpdateCustomerAsync(customer);

                // If the email or username is updated, reflect it in the Identity system
                if (user.Email != model.CustEmail)
                {
                    user.Email = model.CustEmail;
                    user.UserName = model.CustEmail; // Update UserName to email
                    user.NormalizedEmail = model.CustEmail.ToUpper(); // Update NormalizedEmail
                    user.NormalizedUserName = model.CustEmail.ToUpper(); // Update NormalizedUserName

                    var identityUpdateResult = await userManager.UpdateAsync(user);
                    if (!identityUpdateResult.Succeeded)
                    {
                        // Handle errors if user update fails
                        foreach (var error in identityUpdateResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("EditProfile", model); // Return to EditProfile view with error messages
                    }
                }

                // Redirect to Profile page after successful update
                return RedirectToAction("Profile");
            }

            return View("EditProfile", model); // Return to EditProfile view with validation errors if model is invalid
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

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
