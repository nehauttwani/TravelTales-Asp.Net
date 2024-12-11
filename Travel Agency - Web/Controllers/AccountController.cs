using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
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
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel, IFormFile ProfilePicture)
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

                // Handle profile picture upload
                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {
                    // Validate file type and size
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(ProfilePicture.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ProfilePicture", "Only JPG, JPEG, and PNG files are allowed.");
                        return View(registerViewModel);
                    }

                    if (ProfilePicture.Length > 5 * 1024 * 1024) // 5MB size limit
                    {
                        ModelState.AddModelError("ProfilePicture", "The file size must be less than 5 MB.");
                        return View(registerViewModel);
                    }

                    // Save image to wwwroot/images/profile_pictures folder
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile_pictures");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                    var fileName = Guid.NewGuid().ToString() + fileExtension; // Generate a unique file name
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePicture = fileName; // Save the file name to the user object (as a string)
                }

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
                CustEmail = customer.CustEmail,
                ProfilePicture = user.ProfilePicture 
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
                CustEmail = customer.CustEmail,
                ProfilePicture = user.ProfilePicture 
            };

            return View(profileViewModel);
        }

        // POST: Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model, IFormFile ProfilePicture)
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

                // Handle profile picture update
                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {
                    // Validate file type and size
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(ProfilePicture.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ProfilePicture", "Only JPG, JPEG, and PNG files are allowed.");
                        return View("EditProfile", model); // Redirect back to EditProfile view with error
                    }

                    if (ProfilePicture.Length > 5 * 1024 * 1024) // 5MB size limit
                    {
                        ModelState.AddModelError("ProfilePicture", "The file size must be less than 5 MB.");
                        return View("EditProfile", model);
                    }

                    // Save the new image file
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile_pictures");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePicture = fileName; // Save the new file name to the user object
                }

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

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
