    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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
            UserManager<User> userManager)
        {
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
            _customerManager = new CustomerManager(_context);
            _agentsAndAgenciesManager = new AgentsAndAgenciesManager(_context);
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

        public ActionResult Login()
        {
            return View();
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

                    await _customerManager.AddCustomerAsync(customer);

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

        // GET: Account/Profile
        public async Task<ActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            
            if (user != null && user.CustomerId.HasValue)
            {
                var customer = _customerManager.GetCustomer(user.CustomerId.Value);
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
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: Account/EditProfile
        public async Task<ActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null && user.CustomerId.HasValue)
            {
                var customer =  _customerManager.GetCustomer(user.CustomerId.Value);
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
            else {
                return RedirectToAction("Login");
            }
        }

        // POST: Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User); // Get the current user
                if (user != null && user.CustomerId.HasValue)
                {
                    // Update customer information
                    var customer =  _customerManager.GetCustomer(user.CustomerId.Value);
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
                else {
                    return RedirectToAction("Login");
                }
                
            }

            return View("EditProfile", model); // Return to EditProfile view with validation errors if model is invalid
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                ModelState.AddModelError("", "Please select a file");
                return RedirectToAction("Profile");
            }

            var user = await userManager.GetUserAsync(User);

            if (user != null && user.CustomerId.HasValue)
            {
                var customer = _customerManager.GetCustomer(user.CustomerId.Value);
                if (customer == null)
                {
                    return RedirectToAction("Login");
                }

                // Create filename based on CustomerId
                var fileName = $"customer_{customer.CustomerId}__{DateTime.Now.Ticks}.jpg";
                var filePath = Path.Combine("wwwroot", "images", "profile_pictures", fileName);
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile_pictures");

                // Create directory if it doesn't exist
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Save file
                using (var fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), filePath), FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }

                return RedirectToAction("Profile");
            }
            else
            {
                return RedirectToAction("Login");
            }

            
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            TempData["PasswordChange"] = true;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(PasswordOperationsViewModel model)
        {
            ModelState.Remove("Email");
            ModelState.Remove("ResetToken");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.Password!);

            if (result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }



        [HttpGet]
        public IActionResult ResetPassword(string? token = "")
        {
            TempData["PasswordChange"] = true;
            return View(new PasswordOperationsViewModel { ResetToken = token });
        }

        // POST: AccountController/Login
        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordOperationsViewModel model)
        {
            ModelState.Remove("CurrentPassword");
            ModelState.Remove("ResetToken");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No user exists with this email address.");
                return View(model);
            }
            model.ResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, model.ResetToken, model.Password!);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your password has been reset successfully.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

    }
}
