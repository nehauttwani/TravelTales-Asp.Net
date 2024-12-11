using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data.ViewModels;
using Travel_Agency___Data;

namespace Travel_Agency___Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly TravelExpertsContext _context;
        private readonly CustomerManager _customerManager;
        private readonly AgentsAndAgenciesManager _agentsAndAgenciesManager;

        // Identity objects to manage sign-in
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

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
                Agents = _agentsAndAgenciesManager.GetAgents()
            };
            return View(registerViewModel);
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                // Create new user
                var user = new User()
                {
                    UserName = registerViewModel.CustEmail,
                    Email = registerViewModel.CustEmail,
                    FullName = $"{registerViewModel.CustFirstName} {registerViewModel.CustLastName}",
                    PhoneNumber = registerViewModel.CustBusPhone
                };

                // Create user in identity system
                var result = await userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    // Create customer profile
                    var customer = new Customer
                    {
                        CustFirstName = registerViewModel.CustFirstName,
                        CustLastName = registerViewModel.CustLastName,
                        CustAddress = registerViewModel.CustAddress,
                        CustCity = registerViewModel.CustCity,
                        CustProv = registerViewModel.CustProv,
                        CustPostal = registerViewModel.CustPostal,
                        CustCountry = registerViewModel.CustCountry,
                        CustHomePhone = registerViewModel.CustHomePhone,
                        CustBusPhone = registerViewModel.CustBusPhone,
                        CustEmail = registerViewModel.CustEmail
                    };

                    // Add customer asynchronously
                    await _customerManager.AddCustomerAsync(customer);

                    // Link customer to user
                    user.CustomerId = customer.CustomerId;
                    await userManager.UpdateAsync(user);

                    // Sign in user
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

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(LoginViewModal loginViewModal)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginViewModal.Username, loginViewModal.Password, loginViewModal.RememberMe, false);
                if (result.Succeeded)
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
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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

        // GET: Account/ChangePassword
        public async Task<ActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var changePasswordViewModel = new ChangePasswordViewModel();
            return View(changePasswordViewModel);
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}
